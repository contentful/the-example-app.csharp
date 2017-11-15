using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TheExampleApp.Configuration;
using Xunit;

namespace TheExampleApp.Tests.Configuration
{
    public class VisitedLessonsManagerTests
    {

        [Fact]
        public void VisitedLessonsManagerShouldReadFromCookieCorrectly()
        {
            //Arrange
            var cookieCollection = new Mock<IRequestCookieCollection>();
            cookieCollection.SetupGet(c => c["ContentfulVisitedLessons"]).Returns("lesson-x;lesson-y;lesson-z;");
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(r => r.Cookies).Returns(cookieCollection.Object);
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupGet(c => c.HttpContext).Returns(mockContext.Object);

            //Act
            var manager = new VisitedLessonsManager(httpContextAccessor.Object);

            //Assert
            Assert.Collection(manager.VisitedLessons,
                (l) => { Assert.Equal("lesson-x", l); },
                (l) => { Assert.Equal("lesson-y", l); },
                (l) => { Assert.Equal("lesson-z", l); }
            );
        }

        [Fact]
        public void VisitedLessonsManagerShouldNotFailOnEmptyCookie()
        {
            //Arrange
            var cookieCollection = new Mock<IRequestCookieCollection>();
            cookieCollection.SetupGet(c => c["ContentfulVisitedLessons"]).Returns("");
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(r => r.Cookies).Returns(cookieCollection.Object);
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupGet(c => c.HttpContext).Returns(mockContext.Object);

            //Act
            var manager = new VisitedLessonsManager(httpContextAccessor.Object);

            //Assert
            Assert.Empty(manager.VisitedLessons);
        }

        [Fact]
        public void VisitedLessonsManagerShouldAddVisitedLessonCorrectly()
        {
            //Arrange
            var cookieCollection = new Mock<IRequestCookieCollection>();
            cookieCollection.SetupGet(c => c["ContentfulVisitedLessons"]).Returns("lesson-x;lesson-y;lesson-z;");
            var responseCookies = new Mock<IResponseCookies>();
            responseCookies.Setup(c => c.Append("ContentfulVisitedLessons", It.IsAny<string>(), It.IsAny<CookieOptions>())).Verifiable();
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(r => r.Cookies).Returns(cookieCollection.Object);
            var mockResponse = new Mock<HttpResponse>();
            mockResponse.SetupGet(c => c.Cookies).Returns(responseCookies.Object);
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            mockContext.SetupGet(c => c.Response).Returns(mockResponse.Object);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupGet(c => c.HttpContext).Returns(mockContext.Object);
            var manager = new VisitedLessonsManager(httpContextAccessor.Object);

            //Act
            manager.AddVisitedLesson("lesson-bongo");

            //Assert
            Assert.Collection(manager.VisitedLessons,
                (l) => { Assert.Equal("lesson-x", l); },
                (l) => { Assert.Equal("lesson-y", l); },
                (l) => { Assert.Equal("lesson-z", l); },
                (l) => { Assert.Equal("lesson-bongo", l); }
            );
            responseCookies.Verify(c => c.Append("ContentfulVisitedLessons", It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Once);
        }
    }
}
