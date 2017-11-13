using Contentful.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheExampleApp.Configuration;

namespace TheExampleApp.Models
{
    public class BasePageModel : PageModel
    {
        protected readonly IContentfulClient _client;

        public BasePageModel(IContentfulClient client)
        {
            _client = client;
            _client.ContentTypeResolver = new ModulesResolver();
            _client.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
        }
    }
}
