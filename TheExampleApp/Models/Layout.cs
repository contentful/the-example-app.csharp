using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;
namespace TheExampleApp.Models
{
    public class Layout
    {
        public SystemProperties Sys { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public List<ILayoutModule> ContentModules { get; set; }
    }
}
