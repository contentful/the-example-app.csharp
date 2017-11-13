using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheExampleApp.Configuration;
using TheExampleApp.Models;

namespace TheExampleApp.Pages
{
    public class CoursesModel : BasePageModel
    {
        private readonly BreadcrumbsManager _breadcrumbsManager;

        public CoursesModel(IContentfulClient client, BreadcrumbsManager breadcrumbsManager) : base(client)
        {
            _breadcrumbsManager = breadcrumbsManager;
        }

        public async Task OnGet(string category)
        {
            var categories = await _client.GetEntriesByType("category", QueryBuilder<Category>.New.Include(5).LocaleIs(CultureInfo.CurrentCulture.ToString()));
            var queryBuilder = QueryBuilder<Course>.New.ContentTypeIs("course").Include(5);

            var courses = await _client.GetEntries(queryBuilder);

            Categories = categories.ToList();
            SelectedCategory = category;

            if (!string.IsNullOrEmpty(category))
            {
                Courses = courses.Where(c => c.Categories.Any(x => x.Slug == category.ToLower())).ToList();
                var cat = Categories.FirstOrDefault(c => c.Slug == category.ToLower());
                _breadcrumbsManager.ReplaceCrumbForSlug(category.ToLower().Replace('-', ' '), cat.Title);
            }
            else
            {
                Courses = courses.ToList();
            }
        }

        public string SelectedCategory { get; set; }
        public List<Course> Courses { get; set; }
        public List<Category> Categories { get; set; }
    }
}