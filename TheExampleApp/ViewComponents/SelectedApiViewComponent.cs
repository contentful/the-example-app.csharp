using Contentful.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.ViewComponents
{
    public class SelectedApiViewComponent : ViewComponent
    {
        private readonly IContentfulClient _client;

        public SelectedApiViewComponent(IContentfulClient client)
        {
            _client = client;
        }

        public IViewComponentResult Invoke(string currentPath)
        {
            return View(Tuple.Create(_client.IsPreviewClient, currentPath));
        }
    }
}
