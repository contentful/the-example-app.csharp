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
        private readonly VisitedLessonsManager _visitedLessonsManager;
        private readonly BreadcrumbsManager _breadcrumbsManager;

        public IndexModel(IContentfulClient client, VisitedLessonsManager visitedLessonsManager, BreadcrumbsManager breadcrumbsManager) : base(client)
        {
            _visitedLessonsManager = visitedLessonsManager;
            _breadcrumbsManager = breadcrumbsManager;
        }

        public async Task OnGet(string slug)
        {
            var queryBuilder = QueryBuilder<Course>.New.ContentTypeIs("course").FieldEquals(f => f.Slug, slug?.ToLower()).Include(5).LocaleIs(CultureInfo.CurrentCulture.ToString());
            Course = (await _client.GetEntries(queryBuilder)).FirstOrDefault();
            _visitedLessonsManager.AddVisitedLesson(Course.Sys.Id);
            _breadcrumbsManager.ReplaceCrumbForSlug(Course.Slug.ToLower().Replace('-', ' '), Course.Title);
            ViewData["VisitedLessons"] = _visitedLessonsManager.VisitedLessons;
        }

        public Course Course { get; set; }
    }
}