using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheExampleApp.Configuration;
using TheExampleApp.Pages;
using Xunit;

namespace TheExampleApp.Tests.Pages
{
    public class SettingsModelTests
    {

        [Fact]
        public void CreatingSettingsPageShouldSetOptionsCorrectly()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(new Space { Name = "Bananorama" }));
            var options = new Mock<IContentfulOptionsManager>();
            options.SetupGet(c => c.Options).Returns(new ContentfulOptions { DeliveryApiKey = "fgdg", PreviewApiKey = "thh", SpaceId = "34", UsePreviewApi = false });
            
            //Act
            var model = new SettingsModel(options.Object, client.Object);

            //Assert
            Assert.Equal("fgdg", model.AppOptions.AccessToken);
            Assert.Equal("thh", model.AppOptions.PreviewToken);
            Assert.Equal("34", model.AppOptions.SpaceId);
            Assert.False(model.AppOptions.UsePreviewApi);
        }

        [Fact]
        public async Task GettingSettingsPageShouldDisplayCorrectSpaceName()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(new Space { Name = "Bananorama" }));
            var options = new Mock<IContentfulOptionsManager>();
            options.SetupGet(c => c.Options).Returns(new ContentfulOptions());
            var mockSession = new Mock<ISession>();
            byte[] dummy;
            mockSession.Setup(x => x.TryGetValue("EditorialFeatures", out dummy)).Returns(true);
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor(), modelState);
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var model = new SettingsModel(options.Object, client.Object);
            model.PageContext = pageContext;
            //Act

            await model.OnGet();

            //Assert
            Assert.Equal("Bananorama", model.SpaceName);
        }

        [Fact]
        public void SwitchingApiShouldUpdateOptionsCorrectly()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(new Space { Name = "Bananorama" }));
            var options = new Mock<IContentfulOptionsManager>();
            options.SetupGet(c => c.Options).Returns(new ContentfulOptions());
            var mockSession = new Mock<ISession>();
            mockSession.Setup(c => c.Set(nameof(ContentfulOptions), It.IsAny<byte[]>())).Verifiable();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor(), modelState);
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var model = new SettingsModel(options.Object, client.Object);
            model.PageContext = pageContext;
            //Act

            var res = model.OnPostSwitchApi("cpa", "/previous");

            //Assert
            mockSession.Verify(c => c.Set(nameof(ContentfulOptions), It.Is<byte[]>(b => Encoding.UTF8.GetString(b).Contains("\"UsePreviewApi\":true"))));
            Assert.IsType<RedirectResult>(res);
            Assert.Equal("/previous?api=cpa", (res as RedirectResult).Url);
        }

        [Fact]
        public void UpdatingSettingsWithInvalidModelShouldReturnPage()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(new Space { Name = "Bananorama" }));
            var options = new Mock<IContentfulOptionsManager>();
            options.SetupGet(c => c.Options).Returns(new ContentfulOptions());
            var mockSession = new Mock<ISession>();
            mockSession.Setup(c => c.Set(nameof(ContentfulOptions), It.IsAny<byte[]>())).Verifiable();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("er", "ThisIsAnError");
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor(), modelState);
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var model = new SettingsModel(options.Object, client.Object);
            model.TempData = new Mock<ITempDataDictionary>().Object;
            model.PageContext = pageContext;
            //Act

            var res = model.OnPost(new SelectedOptions());

            //Assert
            Assert.IsType<PageResult>(res);
        }

        [Fact]
        public void UpdatingSettingsWithEditorialFeaturesShouldSetSessionCorrectly()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(new Space { Name = "Bananorama" }));
            var options = new Mock<IContentfulOptionsManager>();
            options.SetupGet(c => c.Options).Returns(new ContentfulOptions());
            var mockSession = new Mock<ISession>();
            mockSession.Setup(c => c.Set(nameof(ContentfulOptions), It.IsAny<byte[]>())).Verifiable();
            mockSession.Setup(c => c.Set("EditorialFeatures", It.IsAny<byte[]>())).Verifiable();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor(), modelState);
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var model = new SettingsModel(options.Object, client.Object);
            model.TempData = new Mock<ITempDataDictionary>().Object;
            model.PageContext = pageContext;
            //Act

            var res = model.OnPost(new SelectedOptions { EnableEditorialFeatures = true });

            //Assert
            Assert.IsType<RedirectToPageResult>(res);
            mockSession.Verify(c => c.Set("EditorialFeatures", It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "Enabled")));
        }

        [Fact]
        public void UpdatingSettingsWithoutEditorialFeaturesShouldSetSessionCorrectly()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(new Space { Name = "Bananorama" }));
            var options = new Mock<IContentfulOptionsManager>();
            options.SetupGet(c => c.Options).Returns(new ContentfulOptions());
            var mockSession = new Mock<ISession>();
            mockSession.Setup(c => c.Set(nameof(ContentfulOptions), It.IsAny<byte[]>())).Verifiable();
            mockSession.Setup(c => c.Set("EditorialFeatures", It.IsAny<byte[]>())).Verifiable();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor(), modelState);
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var model = new SettingsModel(options.Object, client.Object);
            model.TempData = new Mock<ITempDataDictionary>().Object;
            model.PageContext = pageContext;
            //Act

            var res = model.OnPost(new SelectedOptions { EnableEditorialFeatures = false });

            //Assert
            Assert.IsType<RedirectToPageResult>(res);
            mockSession.Verify(c => c.Set("EditorialFeatures", It.Is<byte[]>(b => Encoding.UTF8.GetString(b) == "Disabled")));
        }

        [Fact]
        public void UpdatingSettingsShouldSetSessionOptionsCorrectly()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.SerializerSettings).Returns(new JsonSerializerSettings());
            client.Setup(c => c.GetSpace(default(CancellationToken))).Returns(Task.FromResult(new Space { Name = "Bananorama" }));
            var options = new Mock<IContentfulOptionsManager>();
            options.SetupGet(c => c.Options).Returns(new ContentfulOptions());
            var mockSession = new Mock<ISession>();
            mockSession.Setup(c => c.Set(nameof(ContentfulOptions), It.IsAny<byte[]>())).Verifiable();
            mockSession.Setup(c => c.Set("EditorialFeatures", It.IsAny<byte[]>())).Verifiable();
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var modelState = new ModelStateDictionary();
            var modelMetadataProvider = new EmptyModelMetadataProvider();
            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor(), modelState);
            var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            var pageContext = new PageContext(actionContext)
            {
                ViewData = viewData
            };
            var model = new SettingsModel(options.Object, client.Object);
            model.TempData = new Mock<ITempDataDictionary>().Object;
            model.PageContext = pageContext;
            //Act

            var res = model.OnPost(new SelectedOptions { AccessToken = "afafaf", PreviewToken = "bebebe", SpaceId = "Gargamel" });

            //Assert
            Assert.IsType<RedirectToPageResult>(res);
            mockSession.Verify(c => c.Set(nameof(ContentfulOptions), It.Is<byte[]>(
                b => 
                Encoding.UTF8.GetString(b).Contains("\"DeliveryApiKey\":\"afafaf\"") &&
                Encoding.UTF8.GetString(b).Contains("\"PreviewApiKey\":\"bebebe\"") &&
                Encoding.UTF8.GetString(b).Contains("\"SpaceId\":\"Gargamel\"")
                )));
        }

        [Fact]
        public void SelectedOptionsShouldValidateEmptyValuesCorrectly()
        {
            //Arrange
            var options = new SelectedOptions();
            var localizer = new Mock<IViewLocalizer>();
            localizer.SetupGet(l => l["fieldIsRequiredLabel"]).Returns(new LocalizedHtmlString("fieldIsRequiredLabel", "This field is required my friend!"));
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(c => c.GetService(typeof(IViewLocalizer))).Returns(localizer.Object);
            var context = new ValidationContext(options, serviceProvider.Object, null);
            //Act
            var res = options.Validate(context);
            //Assert
            Assert.Collection(res,
                (v) => {
                    Assert.Equal("SpaceId", v.MemberNames.First());
                    Assert.Equal("This field is required my friend!", v.ErrorMessage);
                },
                (v) => {
                    Assert.Equal("AccessToken", v.MemberNames.First());
                    Assert.Equal("This field is required my friend!", v.ErrorMessage);
                },
                (v) => {
                    Assert.Equal("PreviewToken", v.MemberNames.First());
                    Assert.Equal("This field is required my friend!", v.ErrorMessage);
                }
            );
        }

        [Fact]
        public void SelectedOptionsShouldValidateFaultyValuesCorrectly()
        {
            //Arrange
            var unauthorizedError = @"{
  ""sys"": {
    ""type"": ""Error"",
    ""id"": ""AccessTokenInvalid""
  },
                ""message"": ""The access token you sent could not be found or is invalid."",
                ""requestId"": ""123""
            }";
            var options = new SelectedOptions();
            options.AccessToken = "Faulty";
            options.PreviewToken = "Faulty!";
            options.SpaceId = "Some space";
            var localizer = new Mock<IViewLocalizer>();
            localizer.SetupGet(l => l["deliveryKeyInvalidLabel"]).Returns(new LocalizedHtmlString("deliveryKeyInvalidLabel", "Wrong delivery key!"));
            localizer.SetupGet(l => l["previewKeyInvalidLabel"]).Returns(new LocalizedHtmlString("previewKeyInvalidLabel", "Wrong preview key!"));
            localizer.SetupGet(l => l["spaceOrTokenInvalid"]).Returns(new LocalizedHtmlString("spaceOrTokenInvalid", "Wrong space id!"));
            localizer.SetupGet(l => l["somethingWentWrongLabel"]).Returns(new LocalizedHtmlString("somethingWentWrongLabel", "Error!"));
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(c => c.GetService(typeof(IViewLocalizer))).Returns(localizer.Object);
            var handler = new FakeMessageHandler();
            handler.Responses.Enqueue(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.Unauthorized, Content = new StringContent(unauthorizedError) });
            handler.Responses.Enqueue(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.Unauthorized, Content = new StringContent(unauthorizedError) });
            var httpClient = new HttpClient(handler);
            serviceProvider.Setup(c => c.GetService(typeof(HttpClient))).Returns(httpClient);
            var context = new ValidationContext(options, serviceProvider.Object, null);
            //Act
            var res = options.Validate(context);
            //Assert
            Assert.Collection(res,
                (v) => {
                    Assert.Equal("AccessToken", v.MemberNames.First());
                    Assert.Equal("Wrong delivery key!", v.ErrorMessage);
                },
                (v) => {
                    Assert.Equal("PreviewToken", v.MemberNames.First());
                    Assert.Equal("Wrong preview key!", v.ErrorMessage);
                }
            );
        }

        [Fact]
        public void SelectedOptionsShouldValidate404Correctly()
        {
            //Arrange
            var notfoundError = @"{
  ""sys"": {
    ""type"": ""Error"",
    ""id"": ""NotFound""
  },
  ""message"": ""The resource could not be found."",
  ""details"": {
    ""sys"": {
      ""type"": ""Space""
    }
  },
  ""requestId"": ""435""
}";
            var options = new SelectedOptions();
            options.AccessToken = "Faulty";
            options.PreviewToken = "Faulty!";
            options.SpaceId = "Some space";
            var localizer = new Mock<IViewLocalizer>();
            localizer.SetupGet(l => l["deliveryKeyInvalidLabel"]).Returns(new LocalizedHtmlString("deliveryKeyInvalidLabel", "Wrong delivery key!"));
            localizer.SetupGet(l => l["previewKeyInvalidLabel"]).Returns(new LocalizedHtmlString("previewKeyInvalidLabel", "Wrong preview key!"));
            localizer.SetupGet(l => l["spaceOrTokenInvalid"]).Returns(new LocalizedHtmlString("spaceOrTokenInvalid", "Wrong space id!"));
            localizer.SetupGet(l => l["somethingWentWrongLabel"]).Returns(new LocalizedHtmlString("somethingWentWrongLabel", "Error!"));
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(c => c.GetService(typeof(IViewLocalizer))).Returns(localizer.Object);
            var handler = new FakeMessageHandler();
            handler.Responses.Enqueue(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.NotFound, Content = new StringContent(notfoundError) });
            handler.Responses.Enqueue(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.NotFound, Content = new StringContent(notfoundError) });
            var httpClient = new HttpClient(handler);
            serviceProvider.Setup(c => c.GetService(typeof(HttpClient))).Returns(httpClient);
            var context = new ValidationContext(options, serviceProvider.Object, null);
            //Act
            var res = options.Validate(context);
            //Assert
            Assert.Collection(res,
                (v) => {
                    Assert.Equal("SpaceId", v.MemberNames.First());
                    Assert.Equal("Wrong space id!", v.ErrorMessage);
                },
                (v) => {
                    Assert.Equal("SpaceId", v.MemberNames.First());
                    Assert.Equal("Wrong space id!", v.ErrorMessage);
                }
            );
        }
    }

    public class FakeMessageHandler : HttpClientHandler
    {
        public FakeMessageHandler()
        {
            Responses = new Queue<HttpResponseMessage>();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            VerifyRequest?.Invoke(request);
            VerificationBeforeSend?.Invoke();

            if (Responses.Count > 0)
            {
                return await Task.FromResult(Responses.Dequeue());
            }

            return await Task.FromResult(Response);
        }
        public Action<HttpRequestMessage> VerifyRequest { get; set; }
        public Action VerificationBeforeSend { get; set; }
        public Queue<HttpResponseMessage> Responses { get; set; }
        public HttpResponseMessage Response { get; set; }
    }
}
