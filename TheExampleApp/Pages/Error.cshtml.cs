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
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ContentfulException ContentfulException { get; set; }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var ex = HttpContext?.Features?.Get<IExceptionHandlerFeature>()?.Error;

            if(ex is ContentfulException)
            {
                ContentfulException = ex as ContentfulException;
                RequestId = ContentfulException.RequestId;
            }
        }
    }
}
