using System;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class WorkflowStatusHelper
    {
        public static WorkflowStatus GetWorkflowStatus(this OrganisationViewModel model)
        {
            var result = WorkflowStatus.Blank;
            if (model.Status == EnumStatus.Draft)
            {
                var hasLastPublishedDateTimeUtc = model.LastPublishedTimestampUtc.HasValue && model.LastPublishedTimestampUtc > DateTime.MinValue;
                var isBlank = string.IsNullOrWhiteSpace(model.TrainWithUs) || string.IsNullOrWhiteSpace(model.TrainWithDisability);

                if (hasLastPublishedDateTimeUtc)
                {
                    result = isBlank ? WorkflowStatus.BlankSubsequenceDraft : WorkflowStatus.SubsequenceDraft;
                }
                else
                {
                    result = isBlank ? WorkflowStatus.Blank : WorkflowStatus.InitialDraft;
                }
            }
            else
            {
                result = WorkflowStatus.Published;
            }

            return result;
        }
    }
}
