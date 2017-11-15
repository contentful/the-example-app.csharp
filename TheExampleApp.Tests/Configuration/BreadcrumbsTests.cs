using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheExampleApp.Configuration;
using Xunit;

namespace TheExampleApp.Tests.Configuration
{
    public class BreadcrumbsTests
    {
        [Fact]
        public async Task BreadCrumbsShouldGetAllPartsOfPath()
        {
            //Arrange
            var localizer = new Mock<IViewLocalizer>();
            localizer.SetupGet(p => p["homeLabel"]).Returns(new LocalizedHtmlString("HomeLabel", "Home"));
            localizer.Setup(p => p.GetAllStrings(false)).Returns(new List<LocalizedString>());

            var breadcrumbs = new Breadcrumbs((innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, localizer.Object);
            

            var mockContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(r => r.Path).Returns(new PathString("/Courses/Course-X/Lessons/Lesson-Y"));
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            mockContext.SetupGet(c => c.Items).Returns(new Dictionary<object, object>());
            
            //Act
            await breadcrumbs.Invoke(mockContext.Object);

            //Assert
            Assert.True(mockContext.Object.Items.ContainsKey("breadcrumbs"));
            Assert.Collection((mockContext.Object.Items["breadcrumbs"] as List<Breadcrumb>),
                (b) => {
                    Assert.Equal("Home", b.Label);
                    Assert.Equal("/", b.Path);
                },
                (b) => {
                    Assert.Equal("Courses", b.Label);
                    Assert.Equal("/Courses", b.Path);
                }, 
                (b) => {
                    Assert.Equal("Course X", b.Label);
                    Assert.Equal("/Courses/Course-X", b.Path);
                },
                (b) => {
                    Assert.Equal("Lessons", b.Label);
                    Assert.Equal("/Courses/Course-X/Lessons", b.Path);
                },
                (b) => {
                    Assert.Equal("Lesson Y", b.Label);
                    Assert.Equal("/Courses/Course-X/Lessons/Lesson-Y", b.Path);
                }
                );
        }

        [Fact]
        public async Task BreadCrumbsShouldTranslateLabelsIfAvailable()
        {
            //Arrange
            var localizer = new Mock<IViewLocalizer>();
            localizer.SetupGet(p => p["homeLabel"]).Returns(new LocalizedHtmlString("HomeLabel", "Home"));
            localizer.SetupGet(p => p["coursesLabel"]).Returns(new LocalizedHtmlString("coursesLabel", "Bananas"));
            localizer.Setup(p => p.GetAllStrings(false)).Returns(new List<LocalizedString>() { new LocalizedString("coursesLabel", "Bananas") });

            var breadcrumbs = new Breadcrumbs((innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, localizer.Object);


            var mockContext = new Mock<HttpContext>();
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(r => r.Path).Returns(new PathString("/Courses/Course-X/Lessons/Lesson-Y"));
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            mockContext.SetupGet(c => c.Items).Returns(new Dictionary<object, object>());

            //Act
            await breadcrumbs.Invoke(mockContext.Object);

            //Assert
            Assert.True(mockContext.Object.Items.ContainsKey("breadcrumbs"));
            Assert.Collection((mockContext.Object.Items["breadcrumbs"] as List<Breadcrumb>),
                (b) => {
                    Assert.Equal("Home", b.Label);
                    Assert.Equal("/", b.Path);
                },
                (b) => {
                    Assert.Equal("Bananas", b.Label);
                    Assert.Equal("/Courses", b.Path);
                },
                (b) => {
                    Assert.Equal("Course X", b.Label);
                    Assert.Equal("/Courses/Course-X", b.Path);
                },
                (b) => {
                    Assert.Equal("Lessons", b.Label);
                    Assert.Equal("/Courses/Course-X/Lessons", b.Path);
                },
                (b) => {
                    Assert.Equal("Lesson Y", b.Label);
                    Assert.Equal("/Courses/Course-X/Lessons/Lesson-Y", b.Path);
                }
                );
        }

        [Fact]
        public void BreadCrumbManagerShouldReplaceLabelsCorrectly()
        {
            //Arrange
            var crumbs = new List<Breadcrumb>() {
                    new Breadcrumb
                    {
                        Label = "Course X"
                    },
                    new Breadcrumb
                    {
                        Label = "Course Y"
                    },
                    new Breadcrumb
                    {
                        Label = "Course Z"
                    }
                };
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(c => c.Items).Returns(new Dictionary<object, object>() {
                { "breadcrumbs", crumbs }
            });

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupGet(c => c.HttpContext).Returns(mockContext.Object);
            var manager = new BreadcrumbsManager(httpContextAccessor.Object);

            //Act
            manager.ReplaceCrumbForSlug("Course X", "Grapes");
            manager.ReplaceCrumbForSlug("Course Z", "Whiskey");

            //Assert
            Assert.Collection(crumbs,
                (b) => { Assert.Equal("Grapes", b.Label); },
                (b) => { Assert.Equal("Course Y", b.Label); },
                (b) => { Assert.Equal("Whiskey", b.Label); }
                );
        }
    }
}
