using Contentful.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TheExampleApp.Configuration;
using Xunit;

namespace TheExampleApp.Tests.Configuration
{
    public class ContentfulOptionsManagerTests
    {
        [Fact]
        public void OptionsManagerShouldReturnStoredOptionsIfSessionIsEmpty()
        {
            //Arrange
            var mockOptions = new Mock<IOptions<ContentfulOptions>>();
            mockOptions.Setup(x => x.Value).Returns(new ContentfulOptions() { DeliveryApiKey = "brunsås" });
            var mockSession = new Mock<ISession>();
            byte[] dummy;
            mockSession.Setup(x => x.TryGetValue(nameof(ContentfulOptions), out dummy)).Returns(true);
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupGet(c => c.HttpContext).Returns(mockContext.Object);

            var manager = new ContentfulOptionsManager(mockOptions.Object, httpContextAccessor.Object);

            //Act
            var options = manager.Options;

            //Assert
            Assert.Equal("brunsås", options.DeliveryApiKey);
        }

        [Fact]
        public void OptionsManagerShouldReturnSessionOptionsIfSessionIsAvailable()
        {
            //Arrange
            var mockOptions = new Mock<IOptions<ContentfulOptions>>();
            mockOptions.Setup(x => x.Value).Returns(new ContentfulOptions() { DeliveryApiKey = "brunsås" });
            var mockSession = new Mock<ISession>();
            byte[] dummy = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ContentfulOptions { DeliveryApiKey = "Schönebrunn" }));
            mockSession.Setup(x => x.TryGetValue(nameof(ContentfulOptions), out dummy)).Returns(true);
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(c => c.Session).Returns(mockSession.Object);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupGet(c => c.HttpContext).Returns(mockContext.Object);

            var manager = new ContentfulOptionsManager(mockOptions.Object, httpContextAccessor.Object);

            //Act
            var options = manager.Options;

            //Assert
            Assert.Equal("Schönebrunn", options.DeliveryApiKey);
        }
    }
}
