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

        public bool IsEmpty() =>
            !CourseLength.HasValue &&
            string.IsNullOrEmpty(SalaryDetails);

        public void MapInto(ref CourseEnrichmentModel enrichmentModel)
        {
            var courseLength = CourseLength.HasValue ? CourseLength.Value.ToString() : null;

            enrichmentModel.CourseLength = courseLength;
            enrichmentModel.SalaryDetails = SalaryDetails;
        }

        public IEnumerable<string> CopyFrom(CourseEnrichmentModel model)
        {
            var res = new List<string>();

            if (model == null)
            {
                return res;
            }

            if(Enum.TryParse(model.CourseLength, out CourseLength courseLength))
            {
                CourseLength = courseLength;
                res.Add(CourseEnrichmentFieldNames.CourseLength);
            }
            
            if(!string.IsNullOrEmpty(model.SalaryDetails))
            {
                SalaryDetails = model.SalaryDetails;
                res.Add(CourseEnrichmentFieldNames.SalaryDetails);
            }
            
            return res;            
        }
    }
}
