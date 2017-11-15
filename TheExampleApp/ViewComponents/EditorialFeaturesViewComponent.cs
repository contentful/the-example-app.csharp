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
    public class EditorialFeaturesViewComponent : ViewComponent
    {
        private readonly ContentfulOptions _options;

        public EditorialFeaturesViewComponent(IContentfulOptionsManager optionsManager)
        {
            _options = optionsManager.Options;
        }

        public IViewComponentResult Invoke(SystemProperties sys)
        {
            var model = new EditorialFeaturesModel();
            model.Sys = sys;
            model.FeaturesEnabled = HttpContext.Session.GetString("EditorialFeatures") == "Enabled";
            model.SpaceId = _options.SpaceId;
            model.UsePreviewApi = _options.UsePreviewApi;

            return View(model);
        }
    }

    public class EditorialFeaturesModel
    {
        public SystemProperties Sys { get; set; }
        public bool FeaturesEnabled { get; set; }
        public bool UsePreviewApi { get; set; }
        public string SpaceId { get; set; }
    }
}
