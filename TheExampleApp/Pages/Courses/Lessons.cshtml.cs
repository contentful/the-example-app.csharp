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
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheExampleApp.Configuration;
using TheExampleApp.Models;

namespace TheExampleApp.Pages.Courses
{
    /// <summary>
    /// Model for the courses/{course-slug}/lessons/{lessons-slug} view.
    /// </summary>
    public class LessonsModel : BasePageModel
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
        public LessonsModel(IContentfulClient client, IVisitedLessonsManager visitedLessonsManager, IBreadcrumbsManager breadcrumbsManager, IViewLocalizer localizer) : base(client)
        {
            _visitedLessonsManager = visitedLessonsManager;
            _breadcrumbsManager = breadcrumbsManager;
            _localizer = localizer;
        }

        /// <summary>
        /// Returns the result of getting the view.
        /// </summary>
        /// <param name="slug">The slug of the course to display a lesson for.</param>
        /// <param name="lessonSlug">The slug of the lesson to display.</param>
        /// <returns>The view.</returns>
        public async Task<IActionResult> OnGet(string slug, string lessonSlug)
        {
            var queryBuilder = QueryBuilder<Course>.New.ContentTypeIs("course").FieldEquals(f => f.Slug, slug?.ToLower()).Include(5).LocaleIs(HttpContext?.Session?.GetString(Startup.LOCALE_KEY) ?? CultureInfo.CurrentCulture.ToString());
            Course = (await _client.GetEntries(queryBuilder)).FirstOrDefault();

            if (Course == null)
            {
                // If the course is not found return 404.
                TempData["NotFound"] = _localizer["errorMessage404Course"].Value;
                return NotFound();
            }

            SelectedLesson = Course.Lessons.FirstOrDefault(c => c.Slug == lessonSlug?.ToLower());

            if (SelectedLesson == null)
            {
                // If the lesson is not found, also return a 404.
                TempData["NotFound"] = _localizer["errorMessage404Lesson"].Value;
                return NotFound();
            }

            // Replace the label of the breadcrum with the title of the course....
            _breadcrumbsManager.ReplaceCrumbForSlug(Course.Slug.ToLower().Replace('-', ' '), Course.Title);
            // ...and the lesson.
            _breadcrumbsManager.ReplaceCrumbForSlug(SelectedLesson.Slug.ToLower().Replace('-', ' '), SelectedLesson.Title);
            // Add the current lesson as visited.
            _visitedLessonsManager.AddVisitedLesson(SelectedLesson.Sys.Id);

            // Add the visited lessons and courses to viewdata.
            ViewData["VisitedLessons"] = _visitedLessonsManager.VisitedLessons;

            var systemProperties = new List<SystemProperties> { SelectedLesson.Sys };
            if (SelectedLesson.Modules != null && SelectedLesson.Modules.Any())
            {
                systemProperties.AddRange(SelectedLesson.Modules?.Select(c => c.Sys));
            }
            SystemProperties = systemProperties;

            return Page();
        }

        /// <summary>
        /// The course displaying lessons for.
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// The lesson to display.
        /// </summary>
        public Lesson SelectedLesson { get; set; }

        /// <summary>
        /// All system properties this lesson relies on.
        /// </summary>
        public IEnumerable<SystemProperties> SystemProperties { get; set; }

        /// <summary>
        /// The slug of the next lesson of the course.
        /// </summary>
        public string NextLessonSlug { get => Course.Lessons.SkipWhile(x => x != SelectedLesson).Skip(1).FirstOrDefault()?.Slug; }
    }
}