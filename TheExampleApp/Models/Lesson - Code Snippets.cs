using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace TheExampleApp.Models
{
    /// <summary>
    /// Represents a code snippets module for a lesson.
    /// </summary>
    public class LessonCodeSnippets : ILessonModule
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
        /// The Curl snippet.
        /// </summary>
        public string Curl { get; set; }

        /// <summary>
        /// The .NET snippet.
        /// </summary>
        public string DotNet { get; set; }

        /// <summary>
        /// The JavaScript snippet.
        /// </summary>
        public string Javascript { get; set; }

        /// <summary>
        /// The Java snippet.
        /// </summary>
        public string Java { get; set; }

        /// <summary>
        /// The Java for Android snippet.
        /// </summary>
        public string JavaAndroid { get; set; }

        /// <summary>
        /// The PHP snippet.
        /// </summary>
        public string Php { get; set; }

        /// <summary>
        /// The Python snippet.
        /// </summary>
        public string Python { get; set; }

        /// <summary>
        /// The Ruby snippet.
        /// </summary>
        public string Ruby { get; set; }

        /// <summary>
        /// The Swift snippet.
        /// </summary>
        public string Swift { get; set; }
    }
}
