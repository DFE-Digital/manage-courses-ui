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
            var ex = context.Exception as ManageCoursesApiException;

            if (ex != null && ex.StatusCode.HasValue)
            {
                var statusCode = ex.StatusCode.Value;
                var statusCodeInt = (int)statusCode;
                if (statusCodeInt >= 400 && statusCodeInt <= 404)
                {
                    context.Result = StatusCodeViewResult(context, (int)statusCode);
                    context.ExceptionHandled = true;
                    return;
                }

                if (statusCode == HttpStatusCode.UnavailableForLegalReasons)
                {
                    context.Result = new RedirectToActionResult("AcceptTerms", "Home", null);
                    context.ExceptionHandled = true;
                    return;
                }
            } else {
                // not a ManageCoursesApiException - so make sure AI gets notified despite us handling it here.

                // MS Doc examples instantiate TelemetryClient immediately before use rather than keeping an
                // instance around. It's not clear why but here I'm following this example.
                new TelemetryClient().TrackException(context.Exception);
            }

            _logger.LogError(context.Exception, "Unhandled exception");

            context.Result = StatusCodeViewResult(context, (int)HttpStatusCode.InternalServerError);
            context.ExceptionHandled = true;
        }

        private static ViewResult StatusCodeViewResult(ActionContext context, int statusCode)
        {
            return new ViewResult
            {
                ViewName = "~/Views/Error/Index.cshtml",
                StatusCode = statusCode,
                ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), context.ModelState)
                {
                    Model = statusCode,
                }
            };
        }
    }
}
