using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseSalaryEnrichmentViewModel : ICourseEnrichmentViewModel
    {
        public CourseLength? CourseLength { get; set; }
        public string CourseLengthOther { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for salary")]
        public string SalaryDetails { get; set; }

        public CourseRouteDataViewModel RouteData { get; set; }

        public CourseInfoViewModel CourseInfo { get; set; }

        public bool IsEmpty() =>
            !CourseLength.HasValue &&
            string.IsNullOrEmpty(SalaryDetails);

        public void MapInto(ref CourseEnrichmentModel enrichmentModel)
        {
            var courseLength = CourseLength.HasValue ? CourseLength.Value.ToString() : null;
            enrichmentModel.CourseLengthOther = CourseLengthOther;
            enrichmentModel.CourseLength = courseLength;
            enrichmentModel.SalaryDetails = SalaryDetails;
        }

        public void CopyFrom(CourseEnrichmentModel model)
        {
            if (model == null)
            {
                return;
            }

            if(Enum.TryParse(model.CourseLength, out CourseLength courseLength))
            {
                CourseLength = courseLength;
            }
            if (!string.IsNullOrEmpty(model.CourseLengthOther))
            {
                CourseLengthOther = model.CourseLengthOther;
            }
            if (!string.IsNullOrEmpty(model.SalaryDetails))
            {
                SalaryDetails = model.SalaryDetails;
            }
        }
    }
}
