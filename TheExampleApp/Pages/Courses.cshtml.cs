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
    /// <summary>
    /// Model for the /courses view.
    /// </summary>
    public class CoursesModel : BasePageModel
    {
        private readonly IBreadcrumbsManager _breadcrumbsManager;

        /// <summary>
        /// Instantiates the model.
        /// </summary>
        /// <param name="client">The client used to communicate with the Contentful API.</param>
        /// <param name="breadcrumbsManager">Class that manages which breadcrumbs the view should display.</param>
        public CoursesModel(IContentfulClient client, IBreadcrumbsManager breadcrumbsManager) : base(client)
        {
            _breadcrumbsManager = breadcrumbsManager;
        }

        /// <summary>
        /// Returns the result of getting the view.
        /// </summary>
        /// <param name="category">The optional category to filter courses by.</param>
        /// <returns>The view.</returns>
        public async Task OnGet(string category)
        {
            // Get all categories since they're always displayed in the left hand side of the view.
            var categories = await _client.GetEntriesByType("category", QueryBuilder<Category>.New.Include(5).LocaleIs(CultureInfo.CurrentCulture.ToString()));
            var queryBuilder = QueryBuilder<Course>.New.ContentTypeIs("course").Include(5).LocaleIs(CultureInfo.CurrentCulture.ToString()).OrderBy("-sys.createdAt");

            var courses = await _client.GetEntries(queryBuilder);

            Categories = categories.ToList();
            SelectedCategory = category;

            if (!string.IsNullOrEmpty(category))
            {
                // Filter the courses by the selected category.
                Courses = courses.Where(c => c.Categories.Any(x => x.Slug == category.ToLower())).ToList();
                var cat = Categories.FirstOrDefault(c => c.Slug == category.ToLower());
                // Replace the breadcrumb for the category to the title of the category, which is more readable.
                _breadcrumbsManager.ReplaceCrumbForSlug(category.ToLower().Replace('-', ' '), cat.Title);
            }
            else
            {
                // If we don't have a category, just display all courses.
                Courses = courses.ToList();
            }
        }

        /// <summary>
        /// The currently selected category.
        /// </summary>
        public string SelectedCategory { get; set; }

        /// <summary>
        /// The courses to display.
        /// </summary>
        public List<Course> Courses { get; set; }

        /// <summary>
        /// The categories to display.
        /// </summary>
        public List<Category> Categories { get; set; }
    }
}