using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TheExampleApp.Pages
{
    /// <summary>
    /// Model for the error view.
    /// </summary>
    public class ErrorModel : PageModel
    {
        /// <summary>
        /// The request id of the request that had an error.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Whether or not to display the request id.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>
        /// The ContentfulException, if any, that occurred.
        /// </summary>
        public ContentfulException ContentfulException { get; set; }

        /// <summary>
        /// Displays the error view.
        /// </summary>
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            // Get the last exception that occurred.
            var ex = HttpContext?.Features?.Get<IExceptionHandlerFeature>()?.Error;

            if(ex is ContentfulException)
            {
                ContentfulException = ex as ContentfulException;
                RequestId = ContentfulException.RequestId;
            }
        }
    }
}
