using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace TheExampleApp.Models
{
    public class LayoutHighlightedCourse : ILayoutModule
    {
        public SystemProperties Sys { get; set; }

        public string Title { get; set; }

        public Course Course { get; set; }
    }
}
