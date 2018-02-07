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
    /// <summary>
    /// Model for the /settings view.
    /// </summary>
    public class SettingsModel : PageModel
    {
        private readonly IContentfulOptionsManager _manager;
        private readonly IContentfulClient _client;

        /// <summary>
        /// Instantiates the model.
        /// </summary>
        /// <param name="manager">The class that manages the Contentful configuration for the application.</param>
        /// <param name="client">The client used to communicate with the Contentful API.</param>
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

        /// <summary>
        /// Returns the result of getting the view.
        /// </summary>
        /// <returns>The view.</returns>
        public async Task OnGet()
        {
            var sessionErrors = HttpContext.Session.GetString("SettingsErrors");
            var sessionOptions = HttpContext.Session.GetString("SettingsErrorsOptions");
            HttpContext.Session.Remove("SettingsErrors");
            HttpContext.Session.Remove("SettingsErrorsOptions");
            if (!string.IsNullOrEmpty(sessionErrors))
            {
                var errors = JsonConvert.DeserializeObject<List<ModelStateError>>(sessionErrors);
                var options = JsonConvert.DeserializeObject<SelectedOptions>(sessionOptions);
                foreach (var error in errors)
                {
                    ModelState.AddModelError($"AppOptions.{error.Key}", error.ErrorMessage);
                }
                AppOptions = options;
                TempData["Invalid"] = true;
                return;
            }

            var space = await _client.GetSpace();
            SpaceName = space.Name;
            AppOptions.EnableEditorialFeatures = HttpContext.Session.GetString("EditorialFeatures") == "Enabled";
            IsUsingCustomCredentials = _manager.IsUsingCustomCredentials;
        }

        /// <summary>
        /// Post action to reset session credentials to the default ones.
        /// </summary>
        /// <returns>A redirect back to the settings page.</returns>
        public IActionResult OnPostResetCredentials()
        {
            HttpContext.Session.SetString(nameof(ContentfulOptions), "");
            return RedirectToPage("Settings");
        }

        /// <summary>
        /// Post action from the dropdown to switch api.
        /// </summary>
        /// <param name="api">The api to switch to.</param>
        /// <param name="prevPage">The page that originated the post.</param>
        /// <returns>A redirect result back to the originating page.</returns>
        public IActionResult OnPostSwitchApi(string api, string prevPage)
        {
            // Retain all options except whether to use the preview api or not.
            var options = new ContentfulOptions
            {
                UsePreviewApi = api == "cpa",
                DeliveryApiKey = _manager.Options.DeliveryApiKey,
                SpaceId = _manager.Options.SpaceId,
                PreviewApiKey = _manager.Options.PreviewApiKey,
                MaxNumberOfRateLimitRetries = _manager.Options.MaxNumberOfRateLimitRetries,
                ResolveEntriesSelectively = _manager.Options.ResolveEntriesSelectively,
                ManagementApiKey = _manager.Options.ManagementApiKey
            };


            HttpContext.Session.SetString(nameof(ContentfulOptions), JsonConvert.SerializeObject(options));
            return Redirect($"{prevPage}?api={api}");
        }

        public IActionResult OnGetInvalidCredentials()
        {
            
            return RedirectToPage();
        }

        /// <summary>
        /// Post action from the /settings view.
        /// </summary>
        /// <param name="appOptions">The options provided by the user.</param>
        /// <returns>The updated view or the current view with validation errors.</returns>
        public IActionResult OnPost(SelectedOptions appOptions)
        {
            if (!ModelState.IsValid)
            {
                TempData["Invalid"] = true;
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

            var currentOptions = new ContentfulOptions
            {
                DeliveryApiKey = appOptions.AccessToken,
                SpaceId = appOptions.SpaceId,
                UsePreviewApi = _manager.Options.UsePreviewApi,
                PreviewApiKey = appOptions.PreviewToken,
                MaxNumberOfRateLimitRetries = _manager.Options.MaxNumberOfRateLimitRetries,
                ResolveEntriesSelectively = _manager.Options.ResolveEntriesSelectively,
                ManagementApiKey = _manager.Options.ManagementApiKey
            };

            HttpContext.Session.SetString(nameof(ContentfulOptions), JsonConvert.SerializeObject(currentOptions));
            TempData["Success"] = true;

            return RedirectToPage("Settings");
        }

        /// <summary>
        /// The current options the application is using.
        /// </summary>
        public ContentfulOptions Options { get; set; }

        /// <summary>
        /// The options displayed in the view, editeable by the user.
        /// </summary>
        public SelectedOptions AppOptions { get; set; }

        /// <summary>
        /// The name of the currently connected space.
        /// </summary>
        public string SpaceName { get; set; }

        public bool IsUsingCustomCredentials { get; set; }
    }

    /// <summary>
    /// Class encapsulating the options a user can set through the /settings view.
    /// </summary>
    public class SelectedOptions : IValidatableObject
    {
        /// <summary>
        /// The selected space id.
        /// </summary>
        public string SpaceId { get; set; }
        
        /// <summary>
        /// The delivery API access token for the space.
        /// </summary>
        public string AccessToken { get; set; }
        
        /// <summary>
        /// The preview API access token for the space.
        /// </summary>
        public string PreviewToken { get; set; }

        /// <summary>
        /// Whether or not to use the preview API.
        /// </summary>
        public bool UsePreviewApi { get; set; }

        /// <summary>
        /// Whether or not to enable editorial features.
        /// </summary>
        public bool EnableEditorialFeatures { get; set; }

        /// <summary>
        /// Validation logic for the options.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>An IEnumerable of ValidationResult.</returns>
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
                // We got all required fields. Make a test call to each API to verify.
                var httpClient = validationContext.GetService(typeof(HttpClient)) as HttpClient;

                var contentfulClient = new ContentfulClient(httpClient, AccessToken, PreviewToken, SpaceId);

                foreach(var validationResult in MakeTestCalls(httpClient, contentfulClient, localizer))
                {
                    yield return validationResult;
                }
            }
        }

        private IEnumerable<ValidationResult> MakeTestCalls(HttpClient httpClient, IContentfulClient contentfulClient, IViewLocalizer localizer)
        {
            ValidationResult validationResult = null;

            try
            {
                var space = contentfulClient.GetSpace().Result;
            }
            catch (AggregateException ae)
            {
                ae.Handle((ce) => {
                    if (ce is ContentfulException)
                    {
                        if ((ce as ContentfulException).StatusCode == 401)
                        {
                            validationResult = new ValidationResult(localizer["deliveryKeyInvalidLabel"].Value, new[] { nameof(AccessToken) });
                        }
                        else if ((ce as ContentfulException).StatusCode == 404)
                        {
                            validationResult = new ValidationResult(localizer["spaceOrTokenInvalid"].Value, new[] { nameof(SpaceId) });
                        }
                        else
                        {
                            validationResult = new ValidationResult($"{localizer["somethingWentWrongLabel"].Value}: {ce.Message}", new[] { nameof(AccessToken) });
                        }
                        return true;
                    }
                    return false;
                });

            }

            if (validationResult != null)
            {
                yield return validationResult;
            }

            contentfulClient = new ContentfulClient(httpClient, AccessToken, PreviewToken, SpaceId, true);

            try
            {
                var space = contentfulClient.GetSpace().Result;
            }
            catch (AggregateException ae)
            {
                ae.Handle((ce) => {
                    if (ce is ContentfulException)
                    {

                        if ((ce as ContentfulException).StatusCode == 401)
                        {
                            validationResult = new ValidationResult(localizer["previewKeyInvalidLabel"].Value, new[] { nameof(PreviewToken) });
                        }
                        else if ((ce as ContentfulException).StatusCode == 404)
                        {
                            validationResult = new ValidationResult(localizer["spaceOrTokenInvalid"].Value, new[] { nameof(SpaceId) });
                        }
                        else
                        {
                            validationResult = new ValidationResult($"{localizer["somethingWentWrongLabel"].Value}: {ce.Message}", new[] { nameof(PreviewToken) });
                        }
                        return true;
                    }
                    return false;
                });
            }

            if (validationResult != null)
            {
                yield return validationResult;
            }
        }
    }
}