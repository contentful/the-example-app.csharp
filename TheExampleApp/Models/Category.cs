using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace TheExampleApp.Models
{
    /// <summary>
    /// Represents a category.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The system defined meta data properties.
        /// </summary>
        public SystemProperties Sys { get; set; }

        /// <summary>
        /// The title of the category.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The slug for the category.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// The description of the category.
        /// </summary>
        public string Description { get; set; }
    }
}
