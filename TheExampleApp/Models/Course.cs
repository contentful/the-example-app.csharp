using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace TheExampleApp.Models
{
    /// <summary>
    /// Represents
    /// </summary>
    public class Course
    {
        /// <summary>
        /// The system defined meta data properties.
        /// </summary>
        public SystemProperties Sys { get; set; }

        /// <summary>
        /// The title of the course.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The slug for the course.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The image for the course.
        /// </summary>
        public Asset Image { get; set; }

        /// <summary>
        /// The short description of the course.
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// The description of the course.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The duration of the course in minutes.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The skill level required to complete the course.
        /// </summary>
        public string SkillLevel { get; set; }

        /// <summary>
        /// The lessons this course contain.
        /// </summary>
        public List<Lesson> Lessons { get; set; }

        /// <summary>
        /// The categories this course belongs to.
        /// </summary>
        public List<Category> Categories { get; set; }
    }
}
