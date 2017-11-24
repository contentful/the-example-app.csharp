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

        [Fact]
        public void ClickingOnLocaleShouldSwitchLocale()
        {
            _browser.Navigate().GoToUrl(_basehost);
            var dropdownElement = _browser.FindElementsByCssSelector(".header__controls_dropdown").Last();

            Assert.False(dropdownElement.Displayed);

            _browser.FindElementsByCssSelector(".header__controls_label").Last().Click();

            Assert.True(dropdownElement.Displayed);

            _browser.FindElementByCssSelector(".header__controls_button[value=de-DE]").Click();

            var elementText = _browser.FindElementByCssSelector("h2.module-highlighted-course__title").Text;

            Assert.Equal("Hallo Welt", elementText);

            //reset locale back to english
            _browser.Navigate().GoToUrl($"{_basehost}?locale=en-US");
        }

        [Fact]
        public void SettingsPageShouldDisplayDefaultCredentials()
        {
            _browser.Navigate().GoToUrl($"{_basehost}settings");
            
            var spaceId = _browser.FindElementByCssSelector("#AppOptions_SpaceId").GetAttribute("value");
            var accessToken = _browser.FindElementByCssSelector("#AppOptions_AccessToken").GetAttribute("value");
            var previewToken = _browser.FindElementByCssSelector("#AppOptions_PreviewToken").GetAttribute("value");

            Assert.Equal("qz0n5cdakyl9", spaceId);
            Assert.Equal("df2a18b8a5b4426741408fc95fa4331c7388d502318c44a5b22b167c3c1b1d03", accessToken);
            Assert.Equal("10145c6d864960fdca694014ae5e7bdaa7de514a1b5d7fd8bd24027f90c49bbc", previewToken);
        }

        [Fact]
        public void SettingsPageShouldDisplaySuccessMessageOnPost()
        {
            _browser.Navigate().GoToUrl($"{_basehost}settings");

            Assert.Throws<NoSuchElementException>(() => _browser.FindElementByCssSelector(".status-block .status-block--success"));

            _browser.FindElementByCssSelector(".cta[value='Save settings']").Click();

            Assert.Equal("Changes saved successfully!", _browser.FindElementByCssSelector(".status-block.status-block--success").Text);
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
