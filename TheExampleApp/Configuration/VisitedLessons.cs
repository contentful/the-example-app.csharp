using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Configuration
{
    /// <summary>
    /// Class responsible for keeping track of which lessons and courses have been visited and not.
    /// </summary>
    public class VisitedLessonsManager: IVisitedLessonsManager
    {
        private readonly string _key = "ContentfulVisitedLessons";
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// The lessons and courses that have been visited.
        /// </summary>
        public List<string> VisitedLessons { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="VisitedLessonsManager"/>.
        /// </summary>
        /// <param name="accessor">The http context accessor used to retrieve and read cookies.</param>
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

        /// <summary>
        /// Adds a lesson or course as visited.
        /// </summary>
        /// <param name="lessonId">The id of the course or lesson.</param>
        public void AddVisitedLesson(string lessonId)
        {
            VisitedLessons.Add(lessonId);
            var options = new CookieOptions();
            options.HttpOnly = true;
            options.Expires = DateTime.Now.AddDays(7);
            _accessor.HttpContext.Response.Cookies.Append(_key, string.Join(';', VisitedLessons.Distinct()), options);
        }
    }

    /// <summary>
    /// Interface for <see cref="VisitedLessonsManager"/> to use for unit testing.
    /// </summary>
    public interface IVisitedLessonsManager
    {
        /// <summary>
        /// Adds a lesson or course as visited.
        /// </summary>
        /// <param name="lessonId">The id of the course or lesson.</param>
        void AddVisitedLesson(string lessonId);

        /// <summary>
        /// The lessons and courses that have been visited.
        /// </summary>
        List<string> VisitedLessons { get; }
    }
}
