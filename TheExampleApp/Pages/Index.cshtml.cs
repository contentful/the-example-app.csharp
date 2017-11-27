using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
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
            var queryBuilder = QueryBuilder<Layout>.New.ContentTypeIs("layout").FieldEquals(f => f.Slug, "home").Include(4).LocaleIs(CultureInfo.CurrentCulture.ToString());
            var indexPage = (await _client.GetEntries(queryBuilder)).FirstOrDefault();
            IndexPage = indexPage;
        }

        /// <summary>
        /// The layout to display.
        /// </summary>
        public Layout IndexPage { get; set; }
    }
}
