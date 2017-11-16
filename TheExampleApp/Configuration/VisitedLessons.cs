using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Configuration
{
    public class VisitedLessonsManager: IVisitedLessonsManager
    {
        private readonly string _key = "ContentfulVisitedLessons";
        private readonly IHttpContextAccessor _accessor;

        public List<string> VisitedLessons { get; private set; }

        public VisitedLessonsManager(IHttpContextAccessor accessor)
        {
            VisitedLessons = new List<string>();
            _accessor = accessor;
            var cookie = _accessor.HttpContext.Request.Cookies[_key];
            if (!string.IsNullOrEmpty(cookie))
            {
                VisitedLessons = new List<string>(cookie.Split(';', StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public void AddVisitedLesson(string lessonId)
        {
            VisitedLessons.Add(lessonId);
            var options = new CookieOptions();
            options.HttpOnly = true;
            options.Expires = DateTime.Now.AddDays(7);
            _accessor.HttpContext.Response.Cookies.Append(_key, string.Join(';', VisitedLessons.Distinct()), options);
        }
    }

    public interface IVisitedLessonsManager
    {
        void AddVisitedLesson(string lessonId);
        List<string> VisitedLessons { get; }
    }
}
