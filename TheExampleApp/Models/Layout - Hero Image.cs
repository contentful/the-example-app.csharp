using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;
namespace TheExampleApp.Models
{
    /// <summary>
    /// Represents a hero image module for a layout.
    /// </summary>
    public class LayoutHeroImage : ILayoutModule
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
        /// The background image of the module.
        /// </summary>
        public Asset BackgroundImage { get; set; }
    }
}
