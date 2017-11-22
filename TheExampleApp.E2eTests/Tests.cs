using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using Xunit;

namespace TheExampleApp.E2eTests
{
    public class Tests
    {
        ChromeDriver _browser;

        public Tests()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string>() { "headless" });
            _browser = new ChromeDriver(@"C:\projects\TheExampleApp\TheExampleApp.E2eTests", chromeOptions);
        }

        [Fact]
        public void ClickingOnHelpShouldOpenModal()
        {
            _browser.Navigate().GoToUrl("http://localhost:59989/");
            var elementText = _browser.FindElementByCssSelector("h2.module-highlighted-course__title").Text;
            var modalElement = _browser.FindElementByCssSelector("#about-this-modal");

            Assert.Equal("Hello world", elementText);
            Assert.False(modalElement.Displayed);

            _browser.FindElementByCssSelector("#about-this-modal-trigger").Click();
            Assert.True(modalElement.Displayed);
        }

        [Fact]
        public void SwitchingToGermanShouldPersistOverRequests()
        {
            _browser.Navigate().GoToUrl("http://localhost:59989/?locale=de-DE");
            var elementText = _browser.FindElementByCssSelector("h2.module-highlighted-course__title").Text;

            Assert.Equal("Hallo Welt", elementText);

            _browser.Navigate().GoToUrl("http://localhost:59989/courses");
            var headerText = _browser.FindElementByCssSelector("div.courses h1").Text;

            Assert.Contains("Alle Kurse", headerText);

        }
    }
}
