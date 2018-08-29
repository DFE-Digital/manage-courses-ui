﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseEnrichmentViewModel
    {
        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,400}$", ErrorMessage = "Reduce the word count for details about the course")]
        [Required(ErrorMessage = "Give details about the course")]
        public string AboutCourse { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for details about the interview process")]
        public string InterviewProcess { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,350}$", ErrorMessage = "Reduce the word count for details about school placements")]
        [Required(ErrorMessage = "Give details about school placements")]
        public string HowSchoolPlacementsWork { get; set; }

        // course length and fees
        public CourseLength? CourseLength { get; set; }
        public decimal? FeeUkEu { get; set; }
        public decimal? FeeInternational { get; set; }


        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for fee details")]
        public string FeeDetails { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for financial support")]
        public string FinancialSupport { get; set; }

        // course requirements
        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for details about the required qualifications")]
        [Required(ErrorMessage = "Give details about the required qualifications")]
        public string Qualifications { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for details about personal qualities")]
        public string PersonalQualities { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for details about other requirements")]
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
