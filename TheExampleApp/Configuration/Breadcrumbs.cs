using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Configuration
{
    /// <summary>
    /// Middleware to extract breadcrumbs from the current path.
    /// </summary>
    public class Breadcrumbs
    {
        private readonly RequestDelegate _next;
        private readonly IViewLocalizer _localizer;

        /// <summary>
        /// Initializes a new <see cref="Breadcrumbs"/> middleware component.
        /// </summary>
        /// <param name="next">The next request delegate in the chain of middleware.</param>
        /// <param name="localizer">The localizer used to localize the breadcrumbs.</param>
        public Breadcrumbs(RequestDelegate next, IViewLocalizer localizer)
        {
            _next = next;
            _localizer = localizer;
        }

        /// <summary>
        /// Invokes this middleware.
        /// </summary>
        /// <param name="context">The HttpContext of the request to extract breadcrumbs from.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path;
            var parts = path.ToString().Split("/",StringSplitOptions.RemoveEmptyEntries);
            var items = new List<Breadcrumb>();
            items.Add(new Breadcrumb { Label = _localizer["homeLabel"].Value, Path = "/" });
            var translations = _localizer.GetAllStrings(false);
            foreach (var part in parts)
            {
                var label = part.Replace("-", " ");

                if(translations.Any(c => c.Name == $"{LowerCaseFirstChar(label)}Label"))
                {
                    label = _localizer[$"{LowerCaseFirstChar(label)}Label"].Value;
                }

                items.Add(new Breadcrumb { Label = label, Path = $"/{string.Join('/', parts.Take(Array.IndexOf(parts, part) + 1))}" });
            }

            context.Items["breadcrumbs"] = items;

            await _next(context);
        }

        private string LowerCaseFirstChar(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length == 1)
            {
                return s;
            }
            return Char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }

    /// <summary>
    /// Extension to add <see cref="Breadcrumbs"/> middleware to the middleware pipeline.
    /// </summary>
    public static class BreadcrumbsMiddlewareExtensions
    {
        /// <summary>
        /// Adds <see cref="Breadcrumbs"/> to the middleware pipeline.
        /// </summary>
        /// <param name="builder">The application builder to use.</param>
        /// <returns>The application builder.</returns>
        public static IApplicationBuilder UseBreadcrumbs(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Breadcrumbs>();
        }
    }

    /// <summary>
    /// Encapsulates a single breadcrumb.
    /// </summary>
    public class Breadcrumb
    {
        /// <summary>
        /// The human readable label of the breadcrumb.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The path to navigate to if the breadcrumb is clicked.
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    /// Class used to replace specific labels in the breadcrumbs before displaying them.
    /// </summary>
    public class BreadcrumbsManager : IBreadcrumbsManager
    {
        private readonly IHttpContextAccessor _accessor;
        private List<Breadcrumb> _crumbs;

        /// <summary>
        /// Initializes a new instance of <see cref="BreadcrumbsManager"/>.
        /// </summary>
        /// <param name="accessor">The IHttpContextAccessor used to get access to the HttpContext.</param>
        public BreadcrumbsManager(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _crumbs = accessor.HttpContext.Items["breadcrumbs"] as List<Breadcrumb>;
        }

        /// <summary>
        /// Replaces the label of a <see cref="Breadcrumb"/> for a specific slug with a new label.
        /// </summary>
        /// <param name="slug">The slug or "path" of the breadcrumb that will have its label replaced.</param>
        /// <param name="label">The label to replace the current one with.</param>
        public void ReplaceCrumbForSlug(string slug, string label)
        {
            if(_crumbs.Any(c => c.Label == slug))
            {
                _crumbs.First(c => c.Label == slug).Label = label;
            }
        }
    }

    /// <summary>
    /// Interface for <see cref="BreadcrumbsManager"/> to use for unit testing.
    /// </summary>
    public interface IBreadcrumbsManager
    {
        /// <summary>
        /// Replaces the label of a <see cref="Breadcrumb"/> for a specific slug with a new label.
        /// </summary>
        /// <param name="slug">The slug or "path" of the breadcrumb that will have its label replaced.</param>
        /// <param name="label">The label to replace the current one with.</param>
        void ReplaceCrumbForSlug(string slug, string label);
    }
}
