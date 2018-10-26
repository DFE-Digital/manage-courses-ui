using System;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class WorkflowStatusHelper
    {
        public static WorkflowStatus GetWorkflowStatus(this OrganisationViewModel model)
        {
            if (model.Status == EnumStatus.Published)
            {
                return WorkflowStatus.Published;
            }

            var hasBeenPublishedBefore = model.LastPublishedTimestampUtc.HasValue && model.LastPublishedTimestampUtc > DateTime.MinValue;
            if (hasBeenPublishedBefore)
            {
                return model.IsEmpty() ? WorkflowStatus.BlankSubsequentDraft : WorkflowStatus.SubsequentDraft;
            }

            return model.IsEmpty() ? WorkflowStatus.Blank : WorkflowStatus.InitialDraft;

        }
    }
}
