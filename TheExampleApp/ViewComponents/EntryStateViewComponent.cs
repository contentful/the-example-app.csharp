using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TheExampleApp.Configuration;

namespace TheExampleApp.ViewComponents
{
    public class EntryStateViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;
        private readonly ContentfulOptions _options;

        public EntryStateViewComponent(HttpClient httpClient, IContentfulOptionsManager optionsManager)
        {
            _httpClient = httpClient;
            _options = optionsManager.Options;
        }

        public async Task<IViewComponentResult> InvokeAsync(SystemProperties sys)
        {
            var client = new ContentfulClient(_httpClient, _options.DeliveryApiKey, _options.PreviewApiKey, _options.SpaceId, false);
            EntryStateModel entry = null;

            try
            {
                entry = await client.GetEntry<EntryStateModel>(sys.Id);
            }
            catch { }

            if(entry == null)
            {
                entry = new EntryStateModel { Draft = true };
                return View(entry);
            }
            else if(sys.UpdatedAt != entry.Sys.UpdatedAt)
            {
                entry.PendingChanges = true;
            }

            return View(entry);
        }
    }

    public class EntryStateModel
    {
        public SystemProperties Sys { get; set; }
        public bool Draft { get; set; }
        public bool PendingChanges { get; set; }
    }
}
