using Contentful.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheExampleApp.Configuration
{
    /// <summary>
    /// Class used to configure whether the current session should use the application configuration or options from session.
    /// This to allow injecting different credentials at runtime.
    /// </summary>
    /// <remarks>
    /// This class is normally not needed in your application. It is only present to allow switching credentials at runtime, 
    /// which is not something every application needs.
    /// </remarks>
    public class ContentfulOptionsManager : IContentfulOptionsManager
    {
        private ContentfulOptions _options;
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Initializes a new <see cref="ContentfulOptionsManager"/>.
        /// </summary>
        /// <param name="options">The ContentfulOptions loaded from application configuration.</param>
        /// <param name="accessor">The IHttpContextAccessor used to get access to the HttpContext.</param>
        public ContentfulOptionsManager(IOptions<ContentfulOptions> options, IHttpContextAccessor accessor)
        {
            _options = options.Value;
            _accessor = accessor;
        }

        /// <summary>
        /// Gets the currently configured <see cref="ContentfulOptions"/> either from session, if present, or from the application configuration.
        /// </summary>
        public ContentfulOptions Options {
            get {
                var sessionString = _accessor.HttpContext.Session.GetString(nameof(ContentfulOptions));

                if (!string.IsNullOrEmpty(sessionString))
                {
                    return JsonConvert.DeserializeObject<ContentfulOptions>(sessionString);
                }
                return _options;
            }
        }

        /// <summary>
        /// Whether or not the application is using custom credentials.
        /// </summary>
        public bool IsUsingCustomCredentials
        {
            get
            {
                var sessionString = _accessor.HttpContext.Session.GetString(nameof(ContentfulOptions));

                if (string.IsNullOrEmpty(sessionString))
                {
                    return false;
                }

                var options = JsonConvert.DeserializeObject<ContentfulOptions>(sessionString); ;

                return (options.SpaceId == _options.SpaceId && 
                    options.UsePreviewApi == _options.UsePreviewApi &&
                    options.DeliveryApiKey == _options.DeliveryApiKey &&
                    options.PreviewApiKey == _options.PreviewApiKey) == false;
            }
        }
    }

    /// <summary>
    /// Interface for the <see cref="ContentfulOptionsManager"/> to use for unit testing.
    /// </summary>
    public interface IContentfulOptionsManager
    {
        /// <summary>
        /// Gets the currently configured <see cref="ContentfulOptions"/>.
        /// </summary>
        ContentfulOptions Options
        {
            get;
        }

        /// <summary>
        /// Whether or not the application is using custom credentials.
        /// </summary>
        bool IsUsingCustomCredentials { get; }
    }
}
