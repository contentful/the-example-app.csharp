using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Configuration;
using Contentful.Core.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using TheExampleApp.Configuration;

namespace TheExampleApp.Pages
{
    public class SettingsModel : PageModel
    {
        private readonly IContentfulOptionsManager _manager;
        private readonly IContentfulClient _client;

        public SettingsModel(IContentfulOptionsManager manager, IContentfulClient client)
        {
            Options = manager.Options;
            _client = client;
            _manager = manager;
            AppOptions = new SelectedOptions
            {
                SpaceId = Options.SpaceId,
                UsePreviewApi = Options.UsePreviewApi,
                AccessToken = Options.DeliveryApiKey,
                PreviewToken = Options.PreviewApiKey
            };
        }

        public async Task OnGet()
        {
            var space = await _client.GetSpace();
            SpaceName = space.Name;
            AppOptions.EnableEditorialFeatures = HttpContext.Session.GetString("EditorialFeatures") == "Enabled";
        }

        public IActionResult OnPostSwitchApi(string api, string prevPage)
        {
            var options = new ContentfulOptions();
            options.UsePreviewApi = api == "cpa";
            options.DeliveryApiKey = _manager.Options.DeliveryApiKey;
            options.SpaceId = _manager.Options.SpaceId;
            options.PreviewApiKey = _manager.Options.PreviewApiKey;
            options.MaxNumberOfRateLimitRetries = _manager.Options.MaxNumberOfRateLimitRetries;
            options.ResolveEntriesSelectively = _manager.Options.ResolveEntriesSelectively;
            options.ManagementApiKey = _manager.Options.ManagementApiKey;


            HttpContext.Session.SetString(nameof(ContentfulOptions), JsonConvert.SerializeObject(options));
            return Redirect(prevPage);
        }

        public IActionResult OnPost(SelectedOptions appOptions)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
             
            if (appOptions.EnableEditorialFeatures)
            {
                HttpContext.Session.SetString("EditorialFeatures", "Enabled");
            }
            else
            {
                HttpContext.Session.SetString("EditorialFeatures", "Disabled");
            }

            var currentOptions = new ContentfulOptions(); 
            currentOptions.DeliveryApiKey = appOptions.AccessToken;
            currentOptions.SpaceId = appOptions.SpaceId;
            currentOptions.UsePreviewApi = appOptions.UsePreviewApi;
            currentOptions.PreviewApiKey = appOptions.PreviewToken;
            currentOptions.MaxNumberOfRateLimitRetries = _manager.Options.MaxNumberOfRateLimitRetries;
            currentOptions.ResolveEntriesSelectively = _manager.Options.ResolveEntriesSelectively;
            currentOptions.ManagementApiKey = _manager.Options.ManagementApiKey;

            HttpContext.Session.SetString(nameof(ContentfulOptions), JsonConvert.SerializeObject(currentOptions));
            TempData["Success"] = true;

            return RedirectToPage("Settings");
        }

        public ContentfulOptions Options { get; set; }
        public SelectedOptions AppOptions { get; set; }
        public string SpaceName { get; set; }
    }

    public class SelectedOptions : IValidatableObject
    {
        public string SpaceId { get; set; }
        
        public string AccessToken { get; set; }
        
        public string PreviewToken { get; set; }
        public bool UsePreviewApi { get; set; }
        public bool EnableEditorialFeatures { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizer = validationContext.GetService(typeof(IViewLocalizer)) as IViewLocalizer;
            if (string.IsNullOrEmpty(SpaceId))
            {
                yield return new ValidationResult(localizer["fieldIsRequiredLabel"].Value, new[] { nameof(SpaceId) });
            }
            if (string.IsNullOrEmpty(AccessToken))
            {
                yield return new ValidationResult(localizer["fieldIsRequiredLabel"].Value, new[] { nameof(AccessToken) });
            }
            if (string.IsNullOrEmpty(PreviewToken))
            {
                yield return new ValidationResult(localizer["fieldIsRequiredLabel"].Value, new[] { nameof(PreviewToken) });
            }

            if(!string.IsNullOrEmpty(SpaceId) && !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(PreviewToken))
            {
                //We got all required fields. Make a test call to verify.
                var httpClient = validationContext.GetService(typeof(HttpClient)) as HttpClient;
                var contentfulClient = new ContentfulClient(httpClient, AccessToken, PreviewToken, SpaceId);
                ValidationResult vr = null;
                try
                {
                    var s = contentfulClient.GetSpace().Result;
                }
                catch(AggregateException ae)
                {
                    ae.Handle((ce) => {
                        if(ce is ContentfulException)
                        {
                            if ((ce as ContentfulException).StatusCode == 401)
                            {
                                vr = new ValidationResult(localizer["deliveryKeyInvalidLabel"].Value, new[] { nameof(AccessToken) });
                            }
                            else if ((ce as ContentfulException).StatusCode == 404)
                            {
                                vr = new ValidationResult(localizer["spaceOrTokenInvalid"].Value, new[] { nameof(SpaceId) });
                            }
                            else
                            {
                                vr = new ValidationResult($"{localizer["somethingWentWrongLabel"].Value}: {ce.Message}", new[] { nameof(AccessToken) });
                            }
                            return true;
                        }
                        return false;
                    });
                    
                }

                if(vr != null)
                {
                    yield return vr;
                }

                contentfulClient = new ContentfulClient(httpClient, AccessToken, PreviewToken, SpaceId, true);

                try
                {
                    var s = contentfulClient.GetSpace().Result;
                }
                catch (AggregateException ae)
                {
                    ae.Handle((ce) => {
                        if (ce is ContentfulException)
                        {
                            if ((ce as ContentfulException).StatusCode == 401)
                            {
                                vr = new ValidationResult(localizer["previewKeyInvalidLabel"].Value, new[] { nameof(PreviewToken) });
                            }
                            else if ((ce as ContentfulException).StatusCode == 404)
                            {
                                vr = new ValidationResult(localizer["spaceOrTokenInvalid"].Value, new[] { nameof(SpaceId) });
                            }
                            else
                            {
                                vr = new ValidationResult($"{localizer["somethingWentWrongLabel"].Value}: {ce.Message}", new[] { nameof(PreviewToken) });
                            }
                            return true;
                        }
                        return false;
                    });
                }

                if (vr != null)
                {
                    yield return vr;
                }
            }
        }
    }
}