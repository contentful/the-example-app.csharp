using Contentful.Core;
using Contentful.Core.Models.Management;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.ViewComponents
{
    public class LocalesViewComponent : ViewComponent
    {
        private readonly IContentfulClient _client;

        public LocalesViewComponent(IContentfulClient client)
        {
            _client = client;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var space = await _client.GetSpace();

            var selectedLocale = CultureInfo.CurrentCulture.ToString();

            var localeInfo = new LocalesInfo
            {
                Locales = space.Locales,
                SelectedLocale = space.Locales.FirstOrDefault(c => c.Code == selectedLocale) ?? space.Locales.Single(c => c.Default)
            };

            HttpContext.Session.SetString("locale", localeInfo.SelectedLocale.Code);

            return View(localeInfo);
        }
    }

    public class LocalesInfo
    {
        public List<Locale> Locales { get; set; }
        public Locale SelectedLocale { get; set; }
    }
}
