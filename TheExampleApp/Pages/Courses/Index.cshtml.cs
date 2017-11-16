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

namespace TheExampleApp.Pages.Courses
{
    public class IndexModel : BasePageModel
    {
        private readonly IVisitedLessonsManager _visitedLessonsManager;
        private readonly IBreadcrumbsManager _breadcrumbsManager;

        public IndexModel(IContentfulClient client, IVisitedLessonsManager visitedLessonsManager, IBreadcrumbsManager breadcrumbsManager) : base(client)
        {
            _visitedLessonsManager = visitedLessonsManager;
            _breadcrumbsManager = breadcrumbsManager;
        }

        public async Task<IActionResult> OnGet(string slug)
        {
            var queryBuilder = QueryBuilder<Course>.New.ContentTypeIs("course").FieldEquals(f => f.Slug, slug?.ToLower()).Include(5).LocaleIs(CultureInfo.CurrentCulture.ToString());
            Course = (await _client.GetEntries(queryBuilder)).FirstOrDefault();

            if(Course == null)
            {
                return NotFound();
            }

            _visitedLessonsManager.AddVisitedLesson(Course.Sys.Id);
            _breadcrumbsManager.ReplaceCrumbForSlug(Course.Slug.ToLower().Replace('-', ' '), Course.Title);
            ViewData["VisitedLessons"] = _visitedLessonsManager.VisitedLessons;
            return Page();
        }

        public Course Course { get; set; }
    }
}