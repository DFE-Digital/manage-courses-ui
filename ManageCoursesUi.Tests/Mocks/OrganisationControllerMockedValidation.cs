﻿using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ManageCoursesUi.Tests.Mocks
{
    public class OrganisationControllerMockedValidation : OrganisationController
    {
        public OrganisationControllerMockedValidation(IManageApi manageApi) : base(manageApi) { }
        public override bool TryValidateModel(object model)
        {
            this.ModelState.AddModelError("you", "failed");

            return false;
        }
    }
}