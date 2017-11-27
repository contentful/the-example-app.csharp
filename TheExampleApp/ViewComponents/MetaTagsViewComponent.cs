using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.ViewComponents
{
    /// <summary>
    /// View component for the head meta tags.
    /// </summary>
    public class MetaTagsViewComponent : ViewComponent
    {
        private readonly IViewLocalizer _localizer;

        /// <summary>
        /// Initializes the view component.
        /// </summary>
        /// <param name="localizer">The localizer to localize the title with.</param>
        public MetaTagsViewComponent(IViewLocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// Invokes the view component and returns the view.
        /// </summary>
        /// <param name="title">The optional title to set for the current page.</param>
        /// <returns>The view.</returns>
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
