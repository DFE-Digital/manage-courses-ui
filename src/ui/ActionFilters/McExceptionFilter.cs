using System.Net;
using GovUk.Education.ManageCourses.ApiClient;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;

namespace GovUk.Education.ManageCourses.Ui.ActionFilters
{
    public class McExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public McExceptionFilter(ILogger<McExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var swaggerException = context.Exception as SwaggerException;

            if (swaggerException?.StatusCode >= 400 && swaggerException?.StatusCode <= 404)
            {
                // todo: translate to status code without redirect
                context.Result = new RedirectToActionResult("Index", "Error", new { statusCode = swaggerException?.StatusCode });
                context.ExceptionHandled = true;
            }
            else if (swaggerException?.StatusCode == (int)HttpStatusCode.UnavailableForLegalReasons)
            {
                context.Result = new RedirectToActionResult("AcceptTerms", "Home", null);
                context.ExceptionHandled = true;
            }
            else if (swaggerException == null)
            {
                // not a swagger exception - so make sure AI gets notified despite us handling it here.

                // MS Doc examples instantiate TelemetryClient immediately before use rather than keeping an
                // instance around. It's not clear why but here I'm following this example.
                new TelemetryClient().TrackException(context.Exception);
            }

            _logger.LogError(context.Exception, "Unhandled exception");

            var statusCode = (int)HttpStatusCode.InternalServerError;
            context.Result = new ViewResult
            {
                ViewName = "~/Views/Error/Index.cshtml",
                StatusCode = statusCode,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                {
                    Model = statusCode,
                }
            };
            context.ExceptionHandled = true;
        }
    }
}
