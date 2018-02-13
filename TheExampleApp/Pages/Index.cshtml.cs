using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheExampleApp.Models;

namespace TheExampleApp.Pages
{
    /// <summary>
    /// Model for the index view.
    /// </summary>
    public class IndexModel : BasePageModel
    {
        /// <summary>
        /// Instantiates the model.
        /// </summary>
        /// <param name="client">The client used to communicate with the Contentful API.</param>
        public IndexModel(IContentfulClient client) : base(client)
        {
        }

        /// <summary>
        /// Returns the result of getting the view.
        /// </summary>
        /// <returns>The view.</returns>
        public async Task OnGet()
        {
            var queryBuilder = QueryBuilder<Layout>.New.ContentTypeIs("layout").FieldEquals(f => f.Slug, "home").Include(4).LocaleIs(HttpContext?.Session?.GetString(Startup.LOCALE_KEY) ?? CultureInfo.CurrentCulture.ToString());
            var indexPage = (await _client.GetEntries(queryBuilder)).FirstOrDefault();
            IndexPage = indexPage;
            var systemProperties = new List<SystemProperties> { indexPage.Sys };
            if (indexPage.ContentModules != null && indexPage.ContentModules.Any())
            {
                systemProperties.AddRange(indexPage.ContentModules?.Select(c => c.Sys));
            }
            SystemProperties = systemProperties;
        }

        /// <summary>
        /// The layout to display.
        /// </summary>
        public Layout IndexPage { get; set; }

        /// <summary>
        /// All system properties this layout relies on.
        /// </summary>
        public IEnumerable<SystemProperties> SystemProperties { get; set; }
    }
}
