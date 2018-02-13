using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.ViewComponents
{
    /// <summary>
    /// View component handling the dropdown to change locale.
    /// </summary>
    public class LocalesViewComponent : ViewComponent
    {
        private readonly IContentfulClient _client;

        /// <summary>
        /// Instantiates the view component.
        /// </summary>
        /// <param name="client">The client used to communicate with the Contentful API.</param>
        public LocalesViewComponent(IContentfulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Invokes the view component and returns the result.
        /// </summary>
        /// <returns>The view.</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Space space = null;

            try
            {
                // Try to get the space, if this fails we're probably in a state of bad credentials.
                space = await _client.GetSpace();
            }
            catch
            {
                // Getting the space failed, set some default values.
                space = new Space() {
                    Locales = new List<Locale>()
                    {
                        new Locale
                        {
                            Code = "en-US",
                            Default = true,
                            Name = "U.S. English"
                        }
                    }
                };
            }
            var selectedLocale = HttpContext.Session.GetString(Startup.LOCALE_KEY) ?? CultureInfo.CurrentCulture.ToString();
            var localeInfo = new LocalesInfo
            {
                Locales = space.Locales,
                SelectedLocale = space?.Locales.FirstOrDefault(c => c.Code == selectedLocale) ?? space?.Locales.Single(c => c.Default),
            };

            HttpContext.Session.SetString(Startup.LOCALE_KEY, localeInfo.SelectedLocale?.Code);

            localeInfo.UsePreviewApi = _client.IsPreviewClient;

            return View(localeInfo);
        }
    }

    /// <summary>
    /// Model for the <see cref="LocalesViewComponent"/> view.
    /// </summary>
    public class LocalesInfo
    {
        /// <summary>
        /// The available locales.
        /// </summary>
        public List<Locale> Locales { get; set; }

        /// <summary>
        /// The currently selected locale.
        /// </summary>
        public Locale SelectedLocale { get; set; }

        /// <summary>
        /// Whether or no the Preview API is being used.
        /// </summary>
        public bool UsePreviewApi { get; set; }
    }
}
