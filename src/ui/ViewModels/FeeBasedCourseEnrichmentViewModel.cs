using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class FeeBasedCourseEnrichmentViewModel : BaseCourseEnrichmentViewModel
    {

        [Required(ErrorMessage = "Give details about the fee for UK and EU students")]
        public decimal? FeeUkEu { get; set; }
        public decimal? FeeInternational { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for fee details")]
        public string FeeDetails { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for financial support")]

        public string FinancialSupport { get; set; }
    }
}
