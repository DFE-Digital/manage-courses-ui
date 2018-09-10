using System.Net;
using GovUk.Education.ManageCourses.ApiClient;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GovUk.Education.ManageCourses.Ui.ActionFilters
{
    public class McExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var swaggerException = context.Exception as SwaggerException;

            if (swaggerException?.StatusCode >= 400 && swaggerException?.StatusCode <= 404)
            {
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
        }
    }
}
