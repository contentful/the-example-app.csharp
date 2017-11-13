using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Contentful.AspNetCore;
using Contentful.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TheExampleApp.Configuration;

namespace TheExampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddContentful(Configuration);
            // This would normally not be needed, but since we want to load our ContentfulOptions from memory if they're changed within the application
            // we provide our own implementation logic for the IContentfulClient
            services.AddSingleton<ContentfulOptionsManager>();
            services.AddTransient<IContentfulClient, ContentfulClient>((ip) => {
                var client = ip.GetService<HttpClient>();
                var options = ip.GetService<ContentfulOptionsManager>().Options;
                var contentfulClient = new ContentfulClient(client,
                    options);
                var version = typeof(Startup).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;
                contentfulClient.Application = $"app the-example-app.csharp/{version}; {contentfulClient.Application}";
                return contentfulClient;
            });
            services.AddSingleton<IViewLocalizer, JsonViewLocalizer>();
            services.AddTransient<VisitedLessonsManager>();
            services.AddMvc().AddRazorPagesOptions(
                options => {
                    options.Conventions.AddPageRoute("/Courses", "Courses/Categories/{category?}");
                    options.Conventions.AddPageRoute("/Courses/Index", "Courses/{slug}/lessons");
                    options.Conventions.AddPageRoute("/Courses/Lessons", "Courses/{slug}/lessons/{lessonSlug}");
                });
            services.AddSession();
            services.AddTransient<BreadcrumbsManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                var options = new RewriteOptions().AddRedirectToHttps();

                app.UseRewriter(options);
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            
            app.UseSession();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("de-DE"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider()
                    {
                        QueryStringKey = "locale"
                    },
                    new CustomRequestCultureProvider(async (s) => {

                        var sessionCulture = s.Session.GetString("locale");

                        if (!string.IsNullOrEmpty(sessionCulture))
                        {
                            return new ProviderCultureResult(sessionCulture, sessionCulture);
                        }

                        return null;
                    })
                },
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            app.UseBreadcrumbs();
            app.UseDeeplinks();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
