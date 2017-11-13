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
    public class IndexModel : BasePageModel
    {
        public IndexModel(IContentfulClient client) : base(client)
        {
        }

        public async Task OnGet()
        {
            var queryBuilder = QueryBuilder<Layout>.New.ContentTypeIs("layout").FieldEquals(f => f.Slug, "home").Include(4).LocaleIs(CultureInfo.CurrentCulture.ToString());
            var indexPage = (await _client.GetEntries(queryBuilder)).FirstOrDefault();
            IndexPage = indexPage;
        }

        public Layout IndexPage { get; set; }
    }
}
