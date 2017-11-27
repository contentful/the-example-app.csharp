using Contentful.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.ViewComponents
{
    /// <summary>
    /// View component for the API selector dropdown.
    /// </summary>
    public class SelectedApiViewComponent : ViewComponent
    {
        private readonly IContentfulClient _client;

        /// <summary>
        /// Instantiates the view component.
        /// </summary>
        /// <param name="client">The client used to communicate with the Contentful API.</param>
        public SelectedApiViewComponent(IContentfulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Invokes the view component and returns the result.
        /// </summary>
        /// <param name="currentPath">The path of the current request. Used to redirect the request back after changing the api.</param>
        /// <returns></returns>
        public IViewComponentResult Invoke(string currentPath)
        {
            return View(Tuple.Create(_client.IsPreviewClient, currentPath));
        }
    }
}
