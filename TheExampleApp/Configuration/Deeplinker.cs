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
    public class Deeplinker
    {
        private readonly RequestDelegate _next;
        private readonly IContentfulOptionsManager _manager;

        public Deeplinker(RequestDelegate next, IContentfulOptionsManager manager)
        {
            _next = next;
            _manager = manager;
        }

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

    public static class DeeplinkerMiddlewareExtensions
    {
        public static IApplicationBuilder UseDeeplinks(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Deeplinker>();
        }
    }
}
