using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.ViewComponents
{
    public class MetaTagsViewComponent : ViewComponent
    {
        private readonly IViewLocalizer _localizer;

        public MetaTagsViewComponent(IViewLocalizer localizer)
        {
            _localizer = localizer;
        }

        public IViewComponentResult Invoke(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return View("Default", _localizer["defaultTitle"].Value);
            }

            return View("Default", $"{title} — {_localizer["defaultTitle"].Value}");
        }
    }
}
