using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheExampleApp.Configuration;
using TheExampleApp.Models;
using TheExampleApp.Pages.Courses;
using Xunit;

namespace TheExampleApp.Tests.Pages.Courses
{
    public class LessonsModelTests
    {
        [Fact]
        public async Task PageShouldReturn404IfNoCourseWasFound()
        {

            //Arrange
            var courses = new ContentfulCollection<Course>();
            courses.Items = new List<Course>();
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetEntries(It.IsAny<QueryBuilder<Course>>(), default(CancellationToken)))
                .Returns(Task.FromResult(courses));
            var visitedLessons = new Mock<IVisitedLessonsManager>();
            var breadcrumbsManager = new Mock<IBreadcrumbsManager>();
            var viewlocalizer = new Mock<IViewLocalizer>();
            viewlocalizer.SetupGet(c => c[It.IsAny<string>()]).Returns(new LocalizedHtmlString("bab", "hello"));

            var model = new LessonsModel(client.Object, visitedLessons.Object, breadcrumbsManager.Object, viewlocalizer.Object);
            model.TempData = new Mock<ITempDataDictionary>().Object;

            //Act
            var res = await model.OnGet("somewhat-sluggish", "real-slugger");

            //Assert
            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public async Task PageShouldReturn404IfNoLessonWasFound()
        {
            //Arrange
            var courses = new ContentfulCollection<Course>();
            courses.Items = new List<Course> {
                new Course
                {
                    Slug = "somewhat-sluggish",
                    Lessons = new List<Lesson>()
                }
            };
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetEntries(It.IsAny<QueryBuilder<Course>>(), default(CancellationToken)))
                .Returns(Task.FromResult(courses));
            var visitedLessons = new Mock<IVisitedLessonsManager>();
            var breadcrumbsManager = new Mock<IBreadcrumbsManager>();
            var viewlocalizer = new Mock<IViewLocalizer>();
            viewlocalizer.SetupGet(c => c[It.IsAny<string>()]).Returns(new LocalizedHtmlString("something", "else"));
            var model = new LessonsModel(client.Object, visitedLessons.Object, breadcrumbsManager.Object, viewlocalizer.Object);
            model.TempData = new Mock<ITempDataDictionary>().Object;

            //Act
            var res = await model.OnGet("somewhat-sluggish", "real-slugger");

            //Assert
            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public async Task PageShouldSelectCorrectCourseAndLessonBySlugs()
        {
            //Arrange
            var courses = new ContentfulCollection<Course>();
            courses.Items = new List<Course> {
                new Course
                {
                    Slug = "somewhat-sluggish",
                    Lessons = new List<Lesson>
                    {
                        new Lesson
                        {
                            Sys = new SystemProperties
                            {
                                Id = "434"
                            },
                            Slug = "real-slugger"
                        }
                    }
                }
            };
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetEntries(It.IsAny<QueryBuilder<Course>>(), default(CancellationToken)))
                .Returns(Task.FromResult(courses));
            var visitedLessons = new Mock<IVisitedLessonsManager>();
            var breadcrumbsManager = new Mock<IBreadcrumbsManager>();
            var viewlocalizer = new Mock<IViewLocalizer>();
            var httpContext = new Mock<HttpContext>();
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor(), modelState);
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var model = new LessonsModel(client.Object, visitedLessons.Object, breadcrumbsManager.Object, viewlocalizer.Object);
            model.PageContext = pageContext;

            //Act
            var res = await model.OnGet("somewhat-sluggish", "real-slugger");

            //Assert
            Assert.IsType<PageResult>(res);
            Assert.Equal("somewhat-sluggish", model.Course.Slug);
            Assert.Equal("real-slugger", model.SelectedLesson.Slug);
        }
    }
}
