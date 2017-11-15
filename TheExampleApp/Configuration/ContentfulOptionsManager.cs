using Contentful.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Configuration
{
    public class ContentfulOptionsManager : IContentfulOptionsManager
    {
        private ContentfulOptions _options;
        private readonly IHttpContextAccessor _accessor;
        public ContentfulOptionsManager(IOptions<ContentfulOptions> options, IHttpContextAccessor accessor)
        {
            _options = options.Value;
            _accessor = accessor;
        }

        public ContentfulOptions Options {
            get {
                var sessionString = _accessor.HttpContext.Session.GetString(nameof(ContentfulOptions));

                if (!string.IsNullOrEmpty(sessionString))
                {
                    return JsonConvert.DeserializeObject<ContentfulOptions>(sessionString);
                }
                return _options;
            }
        }
    }
    
}
