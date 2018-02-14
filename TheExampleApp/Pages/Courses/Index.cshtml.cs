using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheExampleApp.Configuration;
using TheExampleApp.Models;

namespace TheExampleApp.Pages.Courses
{
    /// <summary>
    /// Model for the /courses/{slug} view.
    /// </summary>
    public class IndexModel : BasePageModel
    {
        private readonly IVisitedLessonsManager _visitedLessonsManager;
        private readonly IBreadcrumbsManager _breadcrumbsManager;
        private readonly IViewLocalizer _localizer;

        /// <summary>
        /// Instantiates the model.
        /// </summary>
        /// <param name="client">The client used to communicate with the Contentful API.</param>
        /// <param name="visitedLessonsManager">Class manages which lessons and courses have been visited.</param>
        /// <param name="breadcrumbsManager">Class that manages which breadcrumbs the view should display.</param>
        public IndexModel(IContentfulClient client, IVisitedLessonsManager visitedLessonsManager, IBreadcrumbsManager breadcrumbsManager, IViewLocalizer localizer) : base(client)
        {
            _visitedLessonsManager = visitedLessonsManager;
            _breadcrumbsManager = breadcrumbsManager;
            _localizer = localizer;
        }

        /// <summary>
        /// Returns the result of getting the view.
        /// </summary>
        /// <param name="slug">The slug of the course to display.</param>
        /// <returns>The view.</returns>
        public async Task<IActionResult> OnGet(string slug)
        {
            var queryBuilder = QueryBuilder<Course>.New.ContentTypeIs("course").FieldEquals(f => f.Slug, slug?.ToLower())
                .Include(5).LocaleIs(HttpContext?.Session?.GetString(Startup.LOCALE_KEY) ?? CultureInfo.CurrentCulture.ToString());
            var courses = await _client.GetEntries(queryBuilder);

            Course = (await _client.GetEntries(queryBuilder)).FirstOrDefault();

            if(Course == null)
            {
                TempData["NotFound"] = _localizer["errorMessage404Course"].Value;
                // If the course is not found return a 404 result.
                return NotFound();
            }

            // Add the current course as visited.
            _visitedLessonsManager.AddVisitedLesson(Course.Sys.Id);
            // Replace the label of the breadcrum with the title of the course.
            _breadcrumbsManager.ReplaceCrumbForSlug(Course.Slug.ToLower().Replace('-', ' '), Course.Title);
            // Add the visited lessons and courses to viewdata.
            ViewData["VisitedLessons"] = _visitedLessonsManager.VisitedLessons;
            return Page();
        }

        /// <summary>
        /// The course being displayed.
        /// </summary>
        public Course Course { get; set; }
    }
}