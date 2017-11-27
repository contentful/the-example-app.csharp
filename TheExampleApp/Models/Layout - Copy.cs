using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;
namespace TheExampleApp.Models
{
    /// <summary>
    /// Represents a copy module used for a layout.
    /// </summary>
    public class LayoutCopy : ILayoutModule
    {
        /// <summary>
        /// The system defined meta data properties.
        /// </summary>
        public SystemProperties Sys { get; set; }

        /// <summary>
        /// The title of the module.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The headline of the module.
        /// </summary>
        public string Headline { get; set; }

        /// <summary>
        /// The body copy of the module.
        /// </summary>
        public string Copy { get; set; }

        /// <summary>
        /// The call to action title.
        /// </summary>
        public string CtaTitle { get; set; }

        /// <summary>
        /// The call to action link.
        /// </summary>
        public string CtaLink { get; set; }

        /// <summary>
        /// The visual style of this module.
        /// </summary>
        public string VisualStyle { get; set; }
    }
}
