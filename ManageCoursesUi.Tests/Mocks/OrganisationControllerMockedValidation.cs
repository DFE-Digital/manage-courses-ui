using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.Services;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ManageCoursesUi.Tests.Mocks
{
    public class OrganisationControllerMockedValidation : OrganisationController
    {
        public OrganisationControllerMockedValidation(IManageApi manageApi, IFrontendUrlService frontendUrlService) : base(manageApi, frontendUrlService) { }
        public override bool TryValidateModel(object model)
        {
            this.ModelState.AddModelError("you", "failed");

            return false;
        }
    }
}
