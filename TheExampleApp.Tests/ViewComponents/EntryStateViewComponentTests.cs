using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TheExampleApp.Configuration;
using TheExampleApp.Tests.Pages;
using TheExampleApp.ViewComponents;
using Xunit;

namespace TheExampleApp.Tests.ViewComponents
{
    public class EntryStateViewComponentTests
    {
        [Fact]
        public async Task ComponentShouldSetDraftIfEntryIsNull()
        {
            //Arrange
            var handler = new FakeMessageHandler();
            handler.Responses.Enqueue(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.NotFound, Content = new StringContent(@"{""sys"":{""type"":""Array""},""total"":0,""skip"":0,""limit"":100,""items"":[]}") });
            var httpClient = new HttpClient(handler);
            var optionsManager = new Mock<IContentfulOptionsManager>();
            optionsManager.SetupGet(c => c.Options).Returns(new ContentfulOptions());
            var component = new EntryStateViewComponent(httpClient, optionsManager.Object);
            var sysProperties = new List<SystemProperties> { new SystemProperties { Id = "434" } };
            //Act
            var res = await component.InvokeAsync(sysProperties);

            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.True(((res as ViewViewComponentResult).ViewData.Model as EntryStateModel).Draft);
        }

        [Fact]
        public async Task ComponentShouldSetPendingChangesIfUpdatedAtDoesNotMatch()
        {
            //Arrange
            var handler = new FakeMessageHandler();
            handler.Responses.Enqueue(new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.OK, Content = new StringContent(@"{""sys"":{""type"":""Array""},""total"":0,""skip"":0,""limit"":100,""items"":[{""sys"": {""updatedAt"":""2017-10-01""}, ""fields"": {""test"": ""pop""}}]}") });
            var httpClient = new HttpClient(handler);
            var optionsManager = new Mock<IContentfulOptionsManager>();
            optionsManager.SetupGet(c => c.Options).Returns(new ContentfulOptions());
            var component = new EntryStateViewComponent(httpClient, optionsManager.Object);
            var sysProperties = new SystemProperties();
            sysProperties.UpdatedAt = new DateTime(2017, 11, 03);
            sysProperties.Id = "123";
            //Act
            var res = await component.InvokeAsync(new[] { sysProperties });

            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.True(((res as ViewViewComponentResult).ViewData.Model as EntryStateModel).PendingChanges);
        }
    }
}
