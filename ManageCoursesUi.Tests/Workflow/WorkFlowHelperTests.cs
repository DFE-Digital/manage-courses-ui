﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
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

            Assert.AreEqual(status, GovUk.Education.ManageCourses.Ui.ViewModels.Enums.WorkflowStatus.Blank);
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

            Assert.AreEqual(status, GovUk.Education.ManageCourses.Ui.ViewModels.Enums.WorkflowStatus.InitialDraft);
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

            Assert.AreEqual(status, GovUk.Education.ManageCourses.Ui.ViewModels.Enums.WorkflowStatus.Published);
        }

        [Test]
        public void GetWorkflowStatus_SubsequentDraft()
        {
            var model = new OrganisationViewModel
            {
                TrainWithDisability = "TrainWithDisability",
                TrainWithUs = "TrainWithUs",
                Status = EnumStatus.Draft,
                LastPublishedTimestampUtc = DateTime.Now
            };

            var status = model.GetWorkflowStatus();

            Assert.AreEqual(status, GovUk.Education.ManageCourses.Ui.ViewModels.Enums.WorkflowStatus.SubsequentDraft);
        }

                [Test]
        public void GetWorkflowStatus_BlankSubsequentDraft()
        {
            var model = new OrganisationViewModel
            {
                TrainWithDisability = "",
                TrainWithUs = "",
                Status = EnumStatus.Draft,
                LastPublishedTimestampUtc = DateTime.Now
            };

            var status = model.GetWorkflowStatus();

            Assert.AreEqual(status, GovUk.Education.ManageCourses.Ui.ViewModels.Enums.WorkflowStatus.BlankSubsequentDraft);
        }
    }
}
