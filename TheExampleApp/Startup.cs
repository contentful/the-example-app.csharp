using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using TheExampleApp.Configuration;

namespace TheExampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment CurrentEnvironment { get; set; }

        public static List<CultureInfo> SupportedCultures = new List<CultureInfo>
            {
                //When adding supported locales make sure to also add a static translation files for the locale under /wwwroot/locales
                new CultureInfo("en-US"),
                new CultureInfo("de-DE"),
            };

        public const string LOCALE_KEY = "locale";

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddContentful(Configuration);
            if (CurrentEnvironment.IsStaging())
            {
                var host = Environment.GetEnvironmentVariable("StagingHost");
                if (!string.IsNullOrEmpty(host))
                {
                    services.AddSingleton((ip) =>
                    {
                        var stagingHandler = new StagingMessageHandler
                        {
                            StagingHost = host
                        };
                        return new HttpClient(stagingHandler);
                    });
                }
            }

            // This would normally not be needed, but since we want to load our ContentfulOptions from memory if they're changed within the application
            // we provide our own implementation logic for the IContentfulClient
            services.AddSingleton<IContentfulOptionsManager, ContentfulOptionsManager>();
            services.AddTransient<IContentfulClient, ContentfulClient>((ip) => {
                var client = ip.GetService<HttpClient>();
                var options = ip.GetService<IContentfulOptionsManager>().Options;
                var contentfulClient = new ContentfulClient(client,
                    options);
                var version = typeof(Startup).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;
                contentfulClient.Application = $"app the-example-app.csharp/{version}; {contentfulClient.Application}";
                return contentfulClient;
            });
            services.AddSingleton<IViewLocalizer, JsonViewLocalizer>();
            services.AddTransient<IVisitedLessonsManager, VisitedLessonsManager>();
            services.AddMvc().AddRazorPagesOptions(
                options => {
                    options.Conventions.AddPageRoute("/Courses", "Courses/Categories/{category?}");
                    options.Conventions.AddPageRoute("/Courses/Index", "Courses/{slug}/lessons");
                    options.Conventions.AddPageRoute("/Courses/Lessons", "Courses/{slug}/lessons/{lessonSlug}");
                });
            services.AddSession(options => {
                // IdleTimeout is set to a high value to confirm to requirements for this particular application.
                // In your application you should use an IdleTimeout that suits your application needs or stick to the default of 20 minutes.
                options.IdleTimeout = TimeSpan.FromDays(2);
            });
            services.AddTransient<IBreadcrumbsManager, BreadcrumbsManager>();
            var embeddedProvider = new EmbeddedFileProvider(GetType().GetTypeInfo().Assembly);
            services.AddSingleton<IFileProvider>(embeddedProvider);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/Error");
            var options = new RewriteOptions();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                    options.Add((c) => {
                        var request = c.HttpContext.Request;
                        if(request.Headers.ContainsKey("X-Forwarded-Proto") && request.Headers["X-Forwarded-Proto"] == "http")
                        {
                            var response = c.HttpContext.Response;
                            response.StatusCode = StatusCodes.Status301MovedPermanently;
                            c.Result = RuleResult.EndResponse;
                            response.Headers[HeaderNames.Location] = "https://" + request.Host + request.Path + request.QueryString;
                        }
                    } );
                app.UseExceptionHandler("/Error");
            }
            options.AddRedirect("courses/(.*)/lessons$", "/courses/$1");

            app.UseRewriter(options);

            app.UseStaticFiles();
            
            app.UseSession();

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new CustomRequestCultureProvider(async (s) => {

                        if (s.Request.Query.ContainsKey(LOCALE_KEY))
                        {
                            s.Session.SetString(LOCALE_KEY, s.Request.Query[LOCALE_KEY]);
                        }

                        return null;
                    }),
                    new QueryStringRequestCultureProvider()
                    {
                        QueryStringKey = LOCALE_KEY
                    },
                    new CustomRequestCultureProvider(async (s) => {

                        var sessionCulture = s.Session.GetString(LOCALE_KEY);

                        if (!string.IsNullOrEmpty(sessionCulture))
                        {
                            return await Task.FromResult(new ProviderCultureResult(sessionCulture, sessionCulture));
                        }

                        return null;
                    })
                },
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = SupportedCultures,
                SupportedUICultures = SupportedCultures
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

    public class StagingMessageHandler : HttpClientHandler
    {
        public string StagingHost { get; set; }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var regex = new Regex("contentful");
            var req = regex.Replace(request.RequestUri.ToString(), StagingHost, 1);
            request.RequestUri = new Uri(req);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
