using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheExampleApp.Configuration;

namespace TheExampleApp.ViewComponents
{
    /// <summary>
    /// Viev component handling whether or not to display editorial features.
    /// </summary>
    public class EditorialFeaturesViewComponent : ViewComponent
    {
        private readonly ContentfulOptions _options;

        /// <summary>
        /// Instantiates the view component.
        /// </summary>
        /// <param name="optionsManager">The class that manages the Contentful configuration for the application.</param>
        public EditorialFeaturesViewComponent(IContentfulOptionsManager optionsManager)
        {
            _options = optionsManager.Options;
        }

        /// <summary>
        /// Invokes the view component and returns the result.
        /// </summary>
        /// <param name="sys">They system properties of the entry to display editorial features for.</param>
        /// <returns>The view.</returns>
        public IViewComponentResult Invoke(IEnumerable<SystemProperties> sys)
        {
            var model = new EditorialFeaturesModel();
            model.Sys = sys;
            model.FeaturesEnabled = HttpContext.Session.GetString("EditorialFeatures") == "Enabled";
            model.SpaceId = _options.SpaceId;
            model.UsePreviewApi = _options.UsePreviewApi;

            return View(model);
        }
    }

    /// <summary>
    /// Model for the <see cref="EditorialFeaturesViewComponent"/> view.
    /// </summary>
    public class EditorialFeaturesModel
    {
        /// <summary>
        /// The system properties of the entry to display editorial features for.
        /// </summary>
        public IEnumerable<SystemProperties> Sys { get; set; }
        /// <summary>
        /// Whether or not features should be enabled.
        /// </summary>
        public bool FeaturesEnabled { get; set; }
        /// <summary>
        /// Whether or not the preview API should be used.
        /// </summary>
        public bool UsePreviewApi { get; set; }
        /// <summary>
        /// The current space id.
        /// </summary>
        public string SpaceId { get; set; }
    }
}
