using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace TheExampleApp.E2eTests
{
    public class Tests : IDisposable
    {
        ChromeDriver _browser;
        string _basehost = "http://localhost:59990/";

        public Tests()
        {
            var startupAssembly = typeof(Tests).GetTypeInfo().Assembly;
            var root = GetProjectPath("", startupAssembly);
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new List<string>() { "headless" });
            _browser = new ChromeDriver(root, chromeOptions);
        }

        [Fact]
        public void ClickingOnHelpShouldOpenModal()
        {
            _browser.Navigate().GoToUrl(_basehost);
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
            _browser.Navigate().GoToUrl($"{_basehost}?locale=de-DE");
            var elementText = _browser.FindElementByCssSelector("h2.module-highlighted-course__title").Text;

            Assert.Equal("Hallo Welt", elementText);

            _browser.Navigate().GoToUrl($"{_basehost}courses");
            var headerText = _browser.FindElementByCssSelector("div.courses h1").Text;

            Assert.Contains("Alle Kurse", headerText);

            //reset locale back to english
            _browser.Navigate().GoToUrl($"{_basehost}?locale=en-US");
        }

        [Fact]
        public void ClickingOnLocaleShouldShowDropdown()
        {
            _browser.Navigate().GoToUrl(_basehost);
            var dropdownElement = _browser.FindElementsByCssSelector(".header__controls_dropdown").Last();

            Assert.False(dropdownElement.Displayed);

            _browser.FindElementsByCssSelector(".header__controls_label").Last().Click();

            Assert.True(dropdownElement.Displayed);
        }

        // Get the full path to the target project for testing
        private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            var projectName = startupAssembly.GetName().Name;
            var applicationBasePath = System.AppContext.BaseDirectory;
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                    if (projectFileInfo.Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

        public void Dispose()
        {
            _browser.Dispose();
        }
    }
}
