using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseFeesEnrichmentViewModel : ICourseEnrichmentViewModel
    {
        public CourseLength? CourseLength { get; set; }
        public decimal? FeeUkEu { get; set; }
        public decimal? FeeInternational { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for fee details")]
        public string FeeDetails { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for financial support")]
        public string FinancialSupport { get; set; }

        public CourseRouteDataViewModel RouteData { get; set; }

        public CourseInfoViewModel CourseInfo { get; set; }

        public void CopyFrom(CourseEnrichmentModel model)
        {
            if (model == null)
            {
                return;
            }

            if (Enum.TryParse(model.CourseLength ?? "", out CourseLength courseLength))
            {
                CourseLength = courseLength;
            }

            if(model.FeeUkEu.HasValue)
            {
                FeeUkEu = model.FeeUkEu;
            }

            if(model.FeeInternational.HasValue)
            {
                FeeInternational = model.FeeInternational;
            }
            
            if(!string.IsNullOrEmpty(model.FeeDetails))
            {
                FeeDetails = model.FeeDetails;
            }
                        
            if(!string.IsNullOrEmpty(model.FinancialSupport))
            {
                FinancialSupport = model.FinancialSupport;
            }
        }
    }
}
