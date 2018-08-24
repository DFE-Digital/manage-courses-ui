using System;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseEnrichmentViewModel
    {
        public string AboutCourse { get; set; }
        public string InterviewProcess { get; set; }
        public string HowSchoolPlacementsWork { get; set; }
        public string Qualifications { get; set; }
        public string PersonalQualities { get; set; }
        public string OtherRequirements { get; set; }
        public DateTime? DraftLastUpdatedUtc { get; internal set; }
        public DateTime? LastPublishedUtc { get; internal set; }

        public WorkflowStatus DeterminePublicationState()
        {
            if (!DraftLastUpdatedUtc.HasValue)
            {
                return WorkflowStatus.Blank;
            }
            if (!LastPublishedUtc.HasValue || LastPublishedUtc.Value < DraftLastUpdatedUtc.Value)
            {
                return WorkflowStatus.SubsequentDraft;
            }
            else
            {
                return WorkflowStatus.Published;
            }
        }
    }
}
