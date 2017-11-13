using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Configuration
{
    public class Breadcrumbs
    {
        private readonly RequestDelegate _next;
        private readonly IViewLocalizer _localizer;

        public Breadcrumbs(RequestDelegate next, IViewLocalizer localizer)
        {
            _next = next;
            _localizer = localizer;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path;
            var parts = path.ToString().Split("/",StringSplitOptions.RemoveEmptyEntries);
            var items = new List<Breadcrumb>();
            items.Add(new Breadcrumb { Label = _localizer["homeLabel"].Value, Path = "/" });
            var translations = _localizer.GetAllStrings();
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

    public static class BreadcrumbsMiddlewareExtensions
    {
        public static IApplicationBuilder UseBreadcrumbs(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Breadcrumbs>();
        }
    }

    public class Breadcrumb
    {
        public string Label { get; set; }
        public string Path { get; set; }
    }

    public class BreadcrumbsManager
    {
        private readonly IHttpContextAccessor _accessor;
        private List<Breadcrumb> _crumbs;

        public BreadcrumbsManager(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _crumbs = accessor.HttpContext.Items["breadcrumbs"] as List<Breadcrumb>;
        }

        public void ReplaceCrumbForSlug(string slug, string label)
        {
            if(_crumbs.Any(c => c.Label == slug))
            {
                _crumbs.First(c => c.Label == slug).Label = label;
            }
        }
    }
}
