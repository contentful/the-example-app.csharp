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
    /// <summary>
    /// View component handling the state of an entry.
    /// </summary>
    public class EntryStateViewComponent : ViewComponent
    {
        private readonly HttpClient _httpClient;
        private readonly ContentfulOptions _options;

        /// <summary>
        /// Instantiate the view component.
        /// </summary>
        /// <param name="httpClient">The httpclient to use for a separate call to the Contentful API.</param>
        /// <param name="optionsManager">The class that manages the Contentful configuration for the application.</param>
        public EntryStateViewComponent(HttpClient httpClient, IContentfulOptionsManager optionsManager)
        {
            _httpClient = httpClient;
            _options = optionsManager.Options;
        }

        /// <summary>
        /// Invokes the view component and returns the result.
        /// </summary>
        /// <param name="sys">They system properties of the entry to display status for.</param>
        /// <returns>The view.</returns>
        public async Task<IViewComponentResult> InvokeAsync(SystemProperties sys)
        {
            // Create a new client with preview set to false to always get the entry from the delivery API.
            var client = new ContentfulClient(_httpClient, _options.DeliveryApiKey, _options.PreviewApiKey, _options.SpaceId, false);
            EntryStateModel entry = null;

            try
            {
                // Try getting the entry by the specified Id. If it throws or returns null the entry is not published.
                entry = await client.GetEntry<EntryStateModel>(sys.Id);
            }
            catch { }

            if(entry == null)
            {
                // The entry is not published, thus it must be in draft mode.
                entry = new EntryStateModel { Draft = true };
                return View(entry);
            }
            else if(sys.UpdatedAt != entry.Sys.UpdatedAt)
            {
                // The entry is published but the UpdatedAt dates do not match, thus it must have pending changes.
                entry.PendingChanges = true;
            }

            return View(entry);
        }
    }

    /// <summary>
    /// Model for the <see cref="EntryStateViewComponent"/> view.
    /// </summary>
    public class EntryStateModel
    {
        /// <summary>
        /// The system properties of the entry to display status for.
        /// </summary>
        public SystemProperties Sys { get; set; }

        /// <summary>
        /// Whether the entry is a draft or not.
        /// </summary>
        public bool Draft { get; set; }

        /// <summary>
        /// Whether the entry has pending changes or not.
        /// </summary>
        public bool PendingChanges { get; set; }
    }
}
