using Microsoft.AspNetCore.Builder;

namespace GovUk.Education.ManageCourses.Ui
{
    public static class SpuriousAuthCbRequestsAppExtensions
    {
        ///<summary>
        ///    This is a TEMPORARY HACK to redirect /auth/cb requests to root.
        ///    This had been made necessary by some external links that take users to the /auth/cb endpoints 
        ///    Without the necessary parameterisation.
        ///</summary>
        public IApplicationBuilder UseTemporaryRedirectForSpuriousAuthCbRequests(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) => {                
                var isSpuriousAuthCbRequest = 
                    context.Request.Path == new Microsoft.AspNetCore.Http.PathString("/auth/cb")
                    && context.Request.Method == "GET"
                    && !context.Request.Query.ContainsKey("code");

                if (isSpuriousAuthCbRequest)
                {
                    context.Response.StatusCode = 302;
                    context.Response.Headers["Location"] = "/";
                }
                else 
                {
                    await next.Invoke();
                }
            });
        }
    }
}