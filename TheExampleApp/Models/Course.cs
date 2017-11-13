using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace TheExampleApp.Models
{
    public class Course
    {
        public SystemProperties Sys { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public Asset Image { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public int Duration { get; set; }

        public string SkillLevel { get; set; }

        public List<Lesson> Lessons { get; set; }

        public List<Category> Categories { get; set; }
    }
}
