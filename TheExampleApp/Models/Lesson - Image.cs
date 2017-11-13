using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace TheExampleApp.Models
{
    public class LessonImage : ILessonModule
    {
        public SystemProperties Sys { get; set; }

        public string Title { get; set; }

        public Asset Image { get; set; }
    }
}
