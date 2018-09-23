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
            return GetWorkflowStatus(model.Status, model.LastPublishedTimestampUtc, model.IsEmpty());
        }
        public static WorkflowStatus GetWorkflowStatus(this OrganisationViewModelForAbout model)
        {
            return GetWorkflowStatus(model.Status, model.LastPublishedTimestampUtc, model.IsEmpty());
        }
        public static WorkflowStatus GetWorkflowStatus(this OrganisationViewModelForContact model)
        {
            return GetWorkflowStatus(model.Status, model.LastPublishedTimestampUtc, model.IsEmpty());
        }

        private static WorkflowStatus GetWorkflowStatus(EnumStatus status, DateTime? lastPublishedTimestampUtc, bool isEmpty)
        {
            var result = WorkflowStatus.Blank;
            if (status == EnumStatus.Draft)
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
