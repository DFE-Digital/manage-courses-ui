using System.Net;
using GovUk.Education.ManageCourses.ApiClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GovUk.Education.ManageCourses.Ui.ActionFilters
{
    public class McExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is SwaggerException swaggerException)
            {
                if (swaggerException.StatusCode == (int) HttpStatusCode.Unauthorized)
                {
                    context.Result = new RedirectToActionResult("Index", "Error", new { statusCode = 401 });
                    context.ExceptionHandled = true;
                }

                var unavailableForLegalReasons = 451; // core 2.1 only (int) HttpStatusCode.UnavailableForLegalReasons
                if (swaggerException.StatusCode == unavailableForLegalReasons)
                {
                    context.Result = new RedirectToActionResult("AcceptTerms", "Home", null);
                    context.ExceptionHandled = true;
                }
            }
        }
    }
}
