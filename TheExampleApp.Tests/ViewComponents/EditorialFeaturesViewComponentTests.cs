using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheExampleApp.Configuration;
using TheExampleApp.ViewComponents;
using Xunit;

namespace TheExampleApp.Tests.ViewComponents
{
    public class EditorialFeaturesViewComponentTests
    {
        [Fact]
        public void ComponentShouldReadPropertiesFromManagerCorrectly()
        {
            //Arrange
            var manager = new Mock<IContentfulOptionsManager>();
            manager.SetupGet(c => c.Options).Returns(new ContentfulOptions {
                SpaceId = "43425",
                UsePreviewApi = false
            });
            var mockSession = new Mock<ISession>();
            byte[] dummy;
            mockSession.Setup(x => x.TryGetValue("EditorialFeatures", out dummy)).Returns(true);
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var viewContext = new ViewContext();
            viewContext.HttpContext = httpContext.Object;
            var componentContext = new ViewComponentContext();
            componentContext.ViewContext = viewContext;
            var component = new EditorialFeaturesViewComponent(manager.Object);
            component.ViewComponentContext = componentContext;

            //Act
            var res = component.Invoke(new[] { new SystemProperties { Id = "3232" } });

            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.Equal("3232", ((res as ViewViewComponentResult).ViewData.Model as EditorialFeaturesModel).Sys.First().Id);
            Assert.False(((res as ViewViewComponentResult).ViewData.Model as EditorialFeaturesModel).FeaturesEnabled);
        }

        [Fact]
        public void ComponentShouldReadPropertiesFromSessionCorrectly()
        {
            //Arrange
            var manager = new Mock<IContentfulOptionsManager>();
            manager.SetupGet(c => c.Options).Returns(new ContentfulOptions
            {
                SpaceId = "43425",
                UsePreviewApi = false
            });
            var mockSession = new Mock<ISession>();
            byte[] dummy = Encoding.UTF8.GetBytes("Enabled");
            mockSession.Setup(x => x.TryGetValue("EditorialFeatures", out dummy)).Returns(true);
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var viewContext = new ViewContext();
            viewContext.HttpContext = httpContext.Object;
            var componentContext = new ViewComponentContext();
            componentContext.ViewContext = viewContext;
            var component = new EditorialFeaturesViewComponent(manager.Object);
            component.ViewComponentContext = componentContext;

            //Act
            var res = component.Invoke(new[] { new SystemProperties { Id = "3232" } });

            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.Equal("3232", ((res as ViewViewComponentResult).ViewData.Model as EditorialFeaturesModel).Sys.First().Id);
            Assert.True(((res as ViewViewComponentResult).ViewData.Model as EditorialFeaturesModel).FeaturesEnabled);
        }
    }
}
