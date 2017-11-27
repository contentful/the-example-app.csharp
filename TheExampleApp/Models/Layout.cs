using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;
namespace TheExampleApp.Models
{
    /// <summary>
    /// Represents a layout.
    /// </summary>
    public class Layout
    {
        /// <summary>
        /// The system defined meta data properties.
        /// </summary>
        public SystemProperties Sys { get; set; }

        /// <summary>
        /// The title of the layout.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The slug of the layout.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The modules that make up the layout.
        /// </summary>
        public List<ILayoutModule> ContentModules { get; set; }
    }
}
