using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace TheExampleApp.Models
{
    /// <summary>
    /// Represents a lesson.
    /// </summary>
    public class Lesson
    {
        /// <summary>
        /// The system defined meta data properties.
        /// </summary>
        public SystemProperties Sys { get; set; }

        /// <summary>
        /// The title of the lesson.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The slug of the lesson.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The modules that makes up the lesson.
        /// </summary>
        public List<ILessonModule> Modules { get; set; }
    }
}
