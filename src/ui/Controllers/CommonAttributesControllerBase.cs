using GovUk.Education.ManageCourses.UI.ActionFilters;
using GovUk.Education.ManageCourses.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    [ServiceFilter(typeof(AnalyticsAttribute))]
    public abstract class CommonAttributesControllerBase : Controller
    {
    }
}
