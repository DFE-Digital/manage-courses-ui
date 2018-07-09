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
            if (context.Exception is SwaggerException swaggerException
                && swaggerException.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
                context.ExceptionHandled = true;
            }
        }
    }
}
