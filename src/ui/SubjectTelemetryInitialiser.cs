using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace GovUk.Education.ManageCourses.Ui
{
    public class SubjectTelemetryInitialiser : ITelemetryInitializer
    {
        IHttpContextAccessor httpContextAccessor;

        public SubjectTelemetryInitialiser(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var foo = httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                var sub = foo.Claims.FirstOrDefault(x => x.Type == "sub");
                if (sub != null)
                {
                    telemetry.Context.Properties["UserSubjectId"] = sub.Value;
                }
            }
            
        }
    }
}
