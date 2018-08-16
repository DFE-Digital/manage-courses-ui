using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class WorkflowStatusHelper
    {
        public static WorkflowStatus GetWorkflowStatus(this OrganisationViewModel model)
        {
            var result = WorkflowStatus.Blank;
            if (model.Status == Status.Draft)
            {
                var hasLastPublishedDateTimeUtc = model.LastPublishedDateTimeUtc.HasValue;
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
