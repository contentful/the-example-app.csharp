using Contentful.Core;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TheExampleApp.ViewComponents;
using Xunit;

namespace TheExampleApp.Tests.ViewComponents
{
    public class SelectedApiViewComponentTests
    {
        [Fact]
        public void ComponentShouldSetPathAndPreviewCorrectlyForPreviewClient()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            client.SetupGet(c => c.IsPreviewClient).Returns(true);
            var component = new SelectedApiViewComponent(client.Object);
            //Act
            var res = component.Invoke("somePath");
            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.True(((res as ViewViewComponentResult).ViewData.Model as Tuple<bool, string>).Item1);
            Assert.Equal("somePath", ((res as ViewViewComponentResult).ViewData.Model as Tuple<bool, string>).Item2);
        }

        [Fact]
        public void ComponentShouldSetPathAndPreviewCorrectlyForNormalClient()
        {
            //Arrange
            var client = new Mock<IContentfulClient>();
            var component = new SelectedApiViewComponent(client.Object);
            //Act
            var res = component.Invoke("someOtherPath");
            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.False(((res as ViewViewComponentResult).ViewData.Model as Tuple<bool, string>).Item1);
            Assert.Equal("someOtherPath", ((res as ViewViewComponentResult).ViewData.Model as Tuple<bool, string>).Item2);
        }
    }
}
