using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
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
    }
}
