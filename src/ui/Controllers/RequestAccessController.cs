using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;
using GovUk.Education.ManageCourses.Ui.Helpers;

namespace GovUk.Education.ManageCourses.Ui.Controllers
{
    public class RequestAccessController : CommonAttributesControllerBase
    {
        private readonly ManageApi _manageApi;

        public RequestAccessController (ManageApi manageApi)
        {
            _manageApi = manageApi;
        }

        [HttpGet]
        [Route ("request-access")]
        public ViewResult Index ()
        {
            return View (new RequestAccessViewModel ());
        }

        [HttpPost]
        [Route ("request-access")]
        public async Task<ActionResult> Create (RequestAccessViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View ("Index", model);
            }

            await _manageApi.LogAccessRequest (new AccessRequest ()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                Organisation = model.Organisation,
                Reason = model.Reason,
            });

            this.TempData.Set("RequestAccess_To_Name", $"{model.FirstName} {model.LastName}");

            return new RedirectResult ("/");
        }
    }
}
