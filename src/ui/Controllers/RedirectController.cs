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
    [Route("organisation/{providerCode}/courses")]
    public IActionResult LegacyCoursePage(string providerCode)
    {
      return RedirectToAction("Show", "Organisation", new { providerCode = providerCode });
    }
  }
}