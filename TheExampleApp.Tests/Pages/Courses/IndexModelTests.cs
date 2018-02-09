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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheExampleApp.Configuration;
using TheExampleApp.Models;
using TheExampleApp.Pages.Courses;
using Xunit;

namespace TheExampleApp.Tests.Pages.Courses
{
    public class IndexModelTests
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
            viewlocalizer.SetupGet(c => c[It.IsAny<string>()]).Returns(new LocalizedHtmlString("po", "po"));
            var model = new IndexModel(client.Object, visitedLessons.Object, breadcrumbsManager.Object, viewlocalizer.Object);
            model.TempData = new Mock<ITempDataDictionary>().Object;
            //Act
            var res = await model.OnGet("sluggy-slug");

            //Assert
            Assert.IsType<NotFoundResult>(res);
        }

        [Fact]
        public async Task PageShouldSelectCorrectCourseBySlug()
        {
            //Arrange
            var courses = new ContentfulCollection<Course>();
            courses.Items = new List<Course>() {
                new Course
                {
                    Slug = "sluggy-slug",
                    Title = "Yello!",
                    Sys = new SystemProperties
                    {
                        Id = "555"
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
            var model = new IndexModel(client.Object, visitedLessons.Object, breadcrumbsManager.Object, viewlocalizer.Object);
            model.PageContext = pageContext;
            //Act
            var res = await model.OnGet("sluggy-slug");

            //Assert
            Assert.IsType<PageResult>(res);
            Assert.Same(courses.ElementAt(0), model.Course);
        }
    }
}
