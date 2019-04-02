using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    /// <summary>
    /// This model is used for showing the summary page of the organisation Enrichment,
    /// and for publishing. The validation covers required fields. For individual enrichment
    /// editor pages, see:
    /// <see cref="AboutCourseEnrichmentViewModel" /> and 
    /// <see cref="CourseRequirementsEnrichmentViewModel" /> and
    /// <see cref="CourseFeesEnrichmentViewModel" /> and
    /// <see cref="CourseSalaryEnrichmentViewModel" />
    /// </summary>
    public abstract class BaseCourseEnrichmentViewModel
    {
        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,400}$", ErrorMessage = "Reduce the word count for about this course")]
        [Required(ErrorMessage = "Enter details about this course")]
        public string AboutCourse { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for interview process")]
        public string InterviewProcess { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,350}$", ErrorMessage = "Reduce the word count for how school placements work")]
        [Required(ErrorMessage = "Enter details about school placements")]
        public string HowSchoolPlacementsWork { get; set; }


        [Required(ErrorMessage = "Give details about course length")]// course length and fees
        public CourseLength? CourseLength { get; set; }
        public string CourseLengthInput { get; set; }

        // course requirements
        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for qualifications needed")]
        [Required(ErrorMessage = "Enter details for about qualifications needed")]
        public string Qualifications { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for personal qualities")]
        public string PersonalQualities { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for other requirements")]
        public string OtherRequirements { get; set; }

        public DateTime? DraftLastUpdatedUtc { get; internal set; }
        public DateTime? LastPublishedUtc { get; internal set; }

        public WorkflowStatus DeterminePublicationState()
        {
            if (!DraftLastUpdatedUtc.HasValue)
            {
                return WorkflowStatus.Blank;
            }
            if (DraftLastUpdatedUtc.HasValue && !LastPublishedUtc.HasValue) {
                return WorkflowStatus.InitialDraft;
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

        public CourseRouteDataViewModel RouteData { get; set; }
        public List<string> GetAboutCourseFields() => new List<string> { nameof(this.AboutCourse), nameof(this.InterviewProcess), nameof(this.HowSchoolPlacementsWork) };
        public List<string> GetRequirementsFields() => new List<string> { nameof(this.Qualifications), nameof(this.PersonalQualities), nameof(this.OtherRequirements) };
    }
}
