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
            DateTime? lastPublishedTimestampUtc = model.LastPublishedTimestampUtc;
            bool isEmpty = model.IsEmpty();
            var result = WorkflowStatus.Blank;
            if (model.Status == EnumStatus.Draft)
            {
                var hasLastPublishedDateTimeUtc = lastPublishedTimestampUtc.HasValue && lastPublishedTimestampUtc > DateTime.MinValue;

                if (hasLastPublishedDateTimeUtc)
                {
                    result = isEmpty ? WorkflowStatus.BlankSubsequentDraft : WorkflowStatus.SubsequentDraft;
                }
                else
                {
                    result = isEmpty ? WorkflowStatus.Blank : WorkflowStatus.InitialDraft;
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
