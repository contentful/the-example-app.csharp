using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;
namespace TheExampleApp.Models
{
    public class LayoutCopy : ILayoutModule
    {
        public SystemProperties Sys { get; set; }

        public string Title { get; set; }

        public string Headline { get; set; }

        public string Copy { get; set; }

        public string CtaTitle { get; set; }

        public string CtaLink { get; set; }

        public string VisualStyle { get; set; }
    }
}
