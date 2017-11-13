using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace TheExampleApp.Models
{
    public class LessonCodeSnippets : ILessonModule
    {
        public SystemProperties Sys { get; set; }

        public string Title { get; set; }

        public string Curl { get; set; }

        public string DotNet { get; set; }

        public string Javascript { get; set; }

        public string Java { get; set; }

        public string JavaAndroid { get; set; }

        public string Php { get; set; }

        public string Python { get; set; }

        public string Ruby { get; set; }

        public string Swift { get; set; }
    }
}
