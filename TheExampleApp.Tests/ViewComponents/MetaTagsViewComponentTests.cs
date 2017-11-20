using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TheExampleApp.ViewComponents;
using Xunit;

namespace TheExampleApp.Tests.ViewComponents
{
    public class MetaTagsViewComponentTests
    {
        [Theory]
        [InlineData("Bongo")]
        [InlineData("Banana")]
        [InlineData("Hello")]
        public void ComponentShouldSetProvidedTitleAsPartOfViewTitle(string title)
        {
            //Arrange
            var localizer = new Mock<IViewLocalizer>();
            localizer.SetupGet(c => c["defaultTitle"]).Returns(new LocalizedHtmlString("defaultTitle", "Knorr buljong"));
            var component = new MetaTagsViewComponent(localizer.Object);
            //Act
            var res = component.Invoke(title);
            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.Equal($"{title} — Knorr buljong", ((res as ViewViewComponentResult).ViewData.Model as string));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ComponentShouldSetDefaultTitleIfNoneIsProvided(string title)
        {
            //Arrange
            var localizer = new Mock<IViewLocalizer>();
            localizer.SetupGet(c => c["defaultTitle"]).Returns(new LocalizedHtmlString("defaultTitle", "Knorr buljong"));
            var component = new MetaTagsViewComponent(localizer.Object);
            //Act
            var res = component.Invoke(title);
            //Assert
            Assert.IsType<ViewViewComponentResult>(res);
            Assert.Equal("Knorr buljong", ((res as ViewViewComponentResult).ViewData.Model as string));
        }
    }
}
