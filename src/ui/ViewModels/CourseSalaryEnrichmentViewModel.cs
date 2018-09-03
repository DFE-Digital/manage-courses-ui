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

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for salary")]
        public string SalaryDetails { get; set; }

        public CourseRouteDataViewModel RouteData { get; set; }

        public CourseInfoViewModel CourseInfo { get; set; }

        public IEnumerable<string> CopyFrom(CourseEnrichmentModel model)
        {
            if (model == null)
            {
                yield break;
            }

            if(Enum.TryParse(model.CourseLength, out CourseLength courseLength))
            {
                CourseLength = courseLength;
                yield return CourseEnrichmentFieldNames.CourseLength;
            }
            
            if(!string.IsNullOrEmpty(model.SalaryDetails))
            {
                SalaryDetails = model.SalaryDetails;
                yield return CourseEnrichmentFieldNames.SalaryDetails;
            }
        }
    }
}
