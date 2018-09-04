using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
  public class RedirectController : CommonAttributesControllerBase
  {
    [Authorize]
    [Route("organisation/{ucasCode}/courses")]
    public async Task<IActionResult> LegacyCoursePage(string ucasCode)
    {
      return RedirectToAction("Show", "Organisation", new { ucasCode });
    }
  }
}