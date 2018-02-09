using Contentful.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TheExampleApp.Pages;

namespace TheExampleApp.Configuration
{
    /// <summary>
    /// Middleware to set accesskeys, spaceid and enabling editorial features from query string parameters.
    /// </summary>
    public class Deeplinker
    {
        private readonly RequestDelegate _next;
        private readonly IContentfulOptionsManager _manager;

        /// <summary>
        /// Initializes a new <see cref="Deeplinker"/>.
        /// </summary>
        /// <param name="next">The next request delegate in the chain of middleware.</param>
        /// <param name="manager">The <see cref="IContentfulOptionsManager"/> used to read the current options.</param>

        public Deeplinker(RequestDelegate next, IContentfulOptionsManager manager)
        {
            _next = next;
            _manager = manager;
        }

        /// <summary>
        /// Invokes this middleware.
        /// </summary>
        /// <param name="context">The HttpContext of the request to extract breadcrumbs from.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var query = context.Request.Query;

            if (query.ContainsKey("space_id") && query.ContainsKey("preview_token") && query.ContainsKey("delivery_token"))
            {
                var currentOptions = new ContentfulOptions();
                currentOptions.DeliveryApiKey = query["delivery_token"];
                currentOptions.SpaceId = query["space_id"];
                currentOptions.PreviewApiKey = query["preview_token"];
                currentOptions.UsePreviewApi = query.ContainsKey("api") && query["api"] == "cpa";
                currentOptions.MaxNumberOfRateLimitRetries = _manager.Options.MaxNumberOfRateLimitRetries;
                currentOptions.ResolveEntriesSelectively = _manager.Options.ResolveEntriesSelectively;
                currentOptions.ManagementApiKey = _manager.Options.ManagementApiKey;

                var validateableOptions = new SelectedOptions
                {
                    AccessToken = currentOptions.DeliveryApiKey,
                    PreviewToken = currentOptions.PreviewApiKey,
                    SpaceId = currentOptions.SpaceId
                };
                var validationContext = new ValidationContext(validateableOptions, context.RequestServices, null);
                var validationResult = validateableOptions.Validate(validationContext);

                if (validationResult.Any())
                {
                    var modelStateErrors = new List<ModelStateError>();

                    foreach (var result in validationResult)
                    {
                        foreach (var member in result.MemberNames)
                        {
                            modelStateErrors.Add(new ModelStateError { ErrorMessage = result.ErrorMessage, Key = member });
                        }
                    }

                    context.Session.SetString("SettingsErrors", JsonConvert.SerializeObject(modelStateErrors));
                    context.Session.SetString("SettingsErrorsOptions", JsonConvert.SerializeObject(validateableOptions));
                    context.Response.Redirect("/settings");
                    context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Response.Headers.Add("Pragma", "no-cache");
                    return;
                }
                else
                {
                    context.Session.SetString(nameof(ContentfulOptions), JsonConvert.SerializeObject(currentOptions));
                }
            }
            else if (query.ContainsKey("api"))
            {
                var currentOptions = new ContentfulOptions
                {
                    UsePreviewApi = query["api"] == "cpa",
                    DeliveryApiKey = _manager.Options.DeliveryApiKey,
                    SpaceId = _manager.Options.SpaceId,
                    PreviewApiKey = _manager.Options.PreviewApiKey,
                    MaxNumberOfRateLimitRetries = _manager.Options.MaxNumberOfRateLimitRetries,
                    ResolveEntriesSelectively = _manager.Options.ResolveEntriesSelectively,
                    ManagementApiKey = _manager.Options.ManagementApiKey
                };

                context.Session.SetString(nameof(ContentfulOptions), JsonConvert.SerializeObject(currentOptions));
            }

            if (query.ContainsKey("editorial_features") && string.Equals(query["editorial_features"], "enabled", StringComparison.InvariantCultureIgnoreCase))
            { 
                context.Session.SetString("EditorialFeatures", "Enabled");
            }
            else if(query.ContainsKey("editorial_features"))
            {
                context.Session.SetString("EditorialFeatures", "Disabled");
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension to add <see cref="Deeplinker"/> middleware to the middleware pipeline.
    /// </summary>
    public static class DeeplinkerMiddlewareExtensions
    {
        /// <summary>
        /// Adds <see cref="Deeplinker"/> to the middleware pipeline.
        /// </summary>
        /// <param name="builder">The application builder to use.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseDeeplinks(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Deeplinker>();
        }
    }

    /// <summary>
    /// Simple data transfer object to transfer error messages back to the view.
    /// </summary>
    public class ModelStateError
    {
        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The key for the error message.
        /// </summary>
        public string Key { get; set; }
    }
}
