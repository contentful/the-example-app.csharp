using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheExampleApp.ViewComponents;
using Xunit;

namespace TheExampleApp.Tests.ViewComponents
{
    public class LocalesViewComponentTests
    {
        [Fact]
        public async Task ComponentShouldGetTheSelectedLocale()
        {
            //Arrange
            var space = new Space
            {
                Locales = new List<Locale>
                {
                    new Locale
                    {
                        Code = "sv-SE",
                        Default = true
                    },
                    new Locale
                    {
                        Code = "Klingon"
                    }
                }
            };
            var client = new Mock<IContentfulClient>();
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(space));
            Thread.CurrentThread.CurrentCulture = new CultureInfo("sv-SE");
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Set("locale", It.IsAny<byte[]>())).Verifiable();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var viewContext = new ViewContext();
            viewContext.HttpContext = httpContext.Object;
            var componentContext = new ViewComponentContext();
            componentContext.ViewContext = viewContext;

            var component = new LocalesViewComponent(client.Object);
            component.ViewComponentContext = componentContext;
            //Act
            var res = await component.InvokeAsync();

            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.Equal("sv-SE", ((res as ViewViewComponentResult).ViewData.Model as LocalesInfo).SelectedLocale.Code);
            mockSession.Verify(x => x.Set("locale", It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "sv-SE")), Times.Once);
        }

        [Fact]
        public async Task ComponentShouldGetTheDefaultLocaleIfSelectedLocaleDoesNotExist()
        {
            //Arrange
            var space = new Space
            {
                Locales = new List<Locale>
                {
                    new Locale
                    {
                        Code = "sv-SE",
                        Default = true
                    },
                    new Locale
                    {
                        Code = "Klingon"
                    }
                }
            };
            var client = new Mock<IContentfulClient>();
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(space));
            Thread.CurrentThread.CurrentCulture = new CultureInfo("e-US");
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Set("locale", It.IsAny<byte[]>())).Verifiable();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var viewContext = new ViewContext();
            viewContext.HttpContext = httpContext.Object;
            var componentContext = new ViewComponentContext();
            componentContext.ViewContext = viewContext;

            var component = new LocalesViewComponent(client.Object);
            component.ViewComponentContext = componentContext;
            //Act
            var res = await component.InvokeAsync();

            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.Equal("sv-SE", ((res as ViewViewComponentResult).ViewData.Model as LocalesInfo).SelectedLocale.Code);
            mockSession.Verify(x => x.Set("locale", It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "sv-SE")), Times.Once);
        }

        [Fact]
        public async Task ComponentShouldGetEnglishLocaleIfClientThrows()
        {
            //Arrange

            var client = new Mock<IContentfulClient>();
            client.Setup(c => c.GetSpace(default(CancellationToken))).Throws<Exception>();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var mockSession = new Mock<ISession>();
            mockSession.Setup(x => x.Set("locale", It.IsAny<byte[]>())).Verifiable();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var viewContext = new ViewContext();
            viewContext.HttpContext = httpContext.Object;
            var componentContext = new ViewComponentContext();
            componentContext.ViewContext = viewContext;

            var component = new LocalesViewComponent(client.Object);
            component.ViewComponentContext = componentContext;
            //Act
            var res = await component.InvokeAsync();

            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.Equal("en-US", ((res as ViewViewComponentResult).ViewData.Model as LocalesInfo).SelectedLocale.Code);
            mockSession.Verify(x => x.Set("locale", It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "en-US")), Times.Once);
        }
    }
}
