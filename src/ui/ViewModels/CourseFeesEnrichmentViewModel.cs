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
        public string CourseLengthOther { get; set; }
        public decimal? FeeUkEu { get; set; }
        public decimal? FeeInternational { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for fee details")]
        public string FeeDetails { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for financial support")]
        public string FinancialSupport { get; set; }

        public CourseRouteDataViewModel RouteData { get; set; }

        public CourseInfoViewModel CourseInfo { get; set; }

        public bool IsEmpty() => 
            !CourseLength.HasValue && 
            !FeeUkEu.HasValue && 
            !FeeInternational.HasValue && 
            string.IsNullOrEmpty(FeeDetails) &&
            string.IsNullOrEmpty(FinancialSupport);

        public void MapInto(ref CourseEnrichmentModel enrichmentModel)
        {
            var courseLength = CourseLength.HasValue ? CourseLength.Value.ToString() : null;
            enrichmentModel.CourseLengthOther = CourseLengthOther;
            enrichmentModel.CourseLength = courseLength;
            enrichmentModel.FeeUkEu = FeeUkEu;
            enrichmentModel.FeeInternational = FeeInternational;
            enrichmentModel.FeeDetails = FeeDetails;
            enrichmentModel.FinancialSupport = FinancialSupport;
        }

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
