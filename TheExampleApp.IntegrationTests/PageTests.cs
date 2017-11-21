using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace TheExampleApp.IntegrationTests
{
    public class PageTests
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public PageTests()
        {
            //Arrange
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath("", startupAssembly);

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .UseEnvironment("Development")
                .ConfigureAppConfiguration((b, i) => { i.AddJsonFile("appsettings.json"); })
                .ConfigureServices(InitializeServices)
                .UseStartup(typeof(Startup));


            _server = new TestServer(builder);
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task IndexShouldReturn200()
        {
            // Act
            var response = await _client.GetAsync("/");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Contentful Example App",
                responseString);
        }

        [Fact]
        public async Task IndexShouldReturn200ForGerman()
        {
            // Act
            var response = await _client.GetAsync("/?locale=de-DE");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Die Beispielanwendung für Contentful",
                responseString);
        }

        [Fact]
        public async Task CoursesShouldReturn200()
        {
            // Act
            var response = await _client.GetAsync("/courses");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Contentful Example App", responseString);
            Assert.Contains("<h1>All courses", responseString);
        }

        [Fact]
        public async Task CoursesShouldReturn200ForGerman()
        {
            // Act
            var response = await _client.GetAsync("/courses?locale=de-DE");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Die Beispielanwendung für Contentful", responseString);
            Assert.Contains("<h1>Alle Kurse", responseString);
        }

        [Fact]
        public async Task HelloWorldCourseShouldReturn200()
        {
            // Act
            var response = await _client.GetAsync("/courses/hello-world");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Contentful Example App", responseString);
            Assert.Contains("Hello world</h1>", responseString);
        }

        [Fact]
        public async Task HelloWorldCourseShouldReturn200ForGerman()
        {
            // Act
            var response = await _client.GetAsync("/courses/hello-world?locale=de-DE");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Die Beispielanwendung für Contentful", responseString);
            Assert.Contains("Hallo Welt</h1>", responseString);
        }

        [Fact]
        public async Task MissingCourseShouldReturn404()
        {
            // Act
            var response = await _client.GetAsync("/courses/no-such-thing");
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ImprintShouldReturn200()
        {
            // Act
            var response = await _client.GetAsync("/Imprint");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Office Berlin",
                responseString);
        }

        [Fact]
        public async Task ImprintShouldReturn200ForGerman()
        {
            // Act
            var response = await _client.GetAsync("/Imprint?locale=de-DE");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Büro in Berlin",
                responseString);
        }

        [Fact]
        public async Task ArchitextureLessonShouldReturn200()
        {
            // Act
            var response = await _client.GetAsync("/courses/hello-world/lessons/architecture");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Contentful Example App", responseString);
            Assert.Contains("Architecture</h1>", responseString);
        }

        [Fact]
        public async Task ArchitextureLessonShouldReturn200ForGerman()
        {
            // Act
            var response = await _client.GetAsync("/courses/hello-world/lessons/architecture?locale=de-DE");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("Die Beispielanwendung für Contentful", responseString);
            Assert.Contains("Architektur</h1>", responseString);
        }

        [Fact]
        public async Task MissingLessonShouldReturn404()
        {
            // Act
            var response = await _client.GetAsync("/courses/hello-world/lessons/this-doesn-not-exist");
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SettingsShouldReturn200()
        {
            // Act
            var response = await _client.GetAsync("/Settings");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("<h1>Settings</h1>",
                responseString);
        }

        [Fact]
        public async Task SettingsShouldReturn200ForGerman()
        {
            // Act
            var response = await _client.GetAsync("/Settings?locale=de-DE");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("<h1>Einstellungen</h1>",
                responseString);
        }

        [Fact]
        public async Task SettingsShouldTurnOnEditorialFeaturesAndReturn200()
        {
            // Act
            var response = await _client.GetAsync("/Settings?enable_editorial_features=true");
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("<h1>Settings</h1>",
                responseString);
            Assert.Contains(@"<input type=""checkbox"" checked=""checked"" data-val=""true"" data-val-required=""The EnableEditorialFeatures field is required."" id=""AppOptions_EnableEditorialFeatures"" name=""AppOptions.EnableEditorialFeatures"" value=""true"" />",
                responseString);
        }

        [Fact]
        public async Task PostingToSwitchApiShouldSwitchAPIAndReturn302()
        {
            //Arrange
            var formContent = await GetRequestContentAsync(_client, "/Settings", new Dictionary<string, string>
                {
                    { "api", "cpa" } ,
                    { "prevPage", "/" },
                });
            // Act
            var response = await _client.PostAsync("/Settings?handler=SwitchApi", formContent);
            var responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        }

        protected virtual void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());
            services.AddSingleton(manager);
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

        private static async Task<FormUrlEncodedContent> GetRequestContentAsync(HttpClient _client, string path, IDictionary<string, string> data)
        {
            // Make a request for the resource.
            var getResponse = await _client.GetAsync(path);

            // Set the response's antiforgery cookie on the HttpClient.
            _client.DefaultRequestHeaders.Add("Cookie", getResponse.Headers.GetValues("Set-Cookie"));

            // Obtain the request verification token from the response.
            // Any <form> element in the response contains a token, and
            // they're all the same within a single response.
            var responseMarkup = await getResponse.Content.ReadAsStringAsync();
            var regExp_RequestVerificationToken = new Regex("<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(.*?)\" \\/>", RegexOptions.Compiled);
            var matches = regExp_RequestVerificationToken.Matches(responseMarkup);
            // Group[1] represents the captured characters, represented
            // by (.*?) in the Regex pattern string.
            var token = matches?.FirstOrDefault().Groups[1].Value;

            // Add the token to the form data for the request.
            data.Add("__RequestVerificationToken", token);

            return new FormUrlEncodedContent(data);
        }
    }
}
