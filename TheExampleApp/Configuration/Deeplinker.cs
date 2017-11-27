using Contentful.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            
            if(query.ContainsKey("space_id") && query.ContainsKey("preview_token") && query.ContainsKey("delivery_token"))
            {
                var currentOptions = new ContentfulOptions();
                currentOptions.DeliveryApiKey = query["delivery_token"];
                currentOptions.SpaceId = query["space_id"];
                currentOptions.PreviewApiKey = query["preview_token"];
                currentOptions.UsePreviewApi = query.ContainsKey("api") && query["api"] == "cpa";
                currentOptions.MaxNumberOfRateLimitRetries = _manager.Options.MaxNumberOfRateLimitRetries;
                currentOptions.ResolveEntriesSelectively = _manager.Options.ResolveEntriesSelectively;
                currentOptions.ManagementApiKey = _manager.Options.ManagementApiKey;

                context.Session.SetString(nameof(ContentfulOptions), JsonConvert.SerializeObject(currentOptions));
            }

            if (query.ContainsKey("enable_editorial_features"))
            {
                context.Session.SetString("EditorialFeatures", "Enabled");
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
}
