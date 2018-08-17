using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;

namespace ManageCoursesUi.Workflow
{
    [TestFixture]
    public class WorkFlowHelperTests
    {
        [Test]
        public void GetWorkflowStatus_Blank()
        {
            var model = new OrganisationViewModel();

            var status = model.GetWorkflowStatus();

            Assert.AreEqual(status, WorkflowStatus.Blank);
        }

        [Test]
        public void GetWorkflowStatus_InitialDraft()
        {
            var model = new OrganisationViewModel
            {
                TrainWithDisability = "TrainWithDisability",
                TrainWithUs = "TrainWithUs"
            };

            var status = model.GetWorkflowStatus();

            Assert.AreEqual(status, WorkflowStatus.InitialDraft);
        }

        [Test]
        public void GetWorkflowStatus_Published()
        {
            var model = new OrganisationViewModel
            {
                TrainWithDisability = "TrainWithDisability",
                TrainWithUs = "TrainWithUs",
                Status = EnumStatus.Published,
                LastPublishedTimestampUtc = DateTime.Now
            };

            var status = model.GetWorkflowStatus();

            Assert.AreEqual(status, WorkflowStatus.Published);
        }

        [Test]
        public void GetWorkflowStatus_SubsequenceDraft()
        {
            var model = new OrganisationViewModel
            {
                TrainWithDisability = "TrainWithDisability",
                TrainWithUs = "TrainWithUs",
                Status = EnumStatus.Draft,
                LastPublishedTimestampUtc = DateTime.Now
            };

            var status = model.GetWorkflowStatus();

            Assert.AreEqual(status, WorkflowStatus.SubsequenceDraft);
        }

                [Test]
        public void GetWorkflowStatus_BlankSubsequenceDraft()
        {
            var model = new OrganisationViewModel
            {
                TrainWithDisability = "",
                TrainWithUs = "",
                Status = EnumStatus.Draft,
                LastPublishedTimestampUtc = DateTime.Now
            };

            var status = model.GetWorkflowStatus();

            Assert.AreEqual(status, WorkflowStatus.BlankSubsequenceDraft);
        }
        /*
        BlankSubsequenceDraft
        */
    }
}
