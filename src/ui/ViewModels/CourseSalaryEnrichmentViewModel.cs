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
            enrichmentModel.CourseLengthOther = courseLength == "Other" ?  CourseLengthOther : null;//remove this line for a single field

            //enrichmentModel.CourseLength = courseLength == "Other" ? (string.IsNullOrEmpty(CourseLengthOther) ? null : CourseLengthOther) : courseLength;
            //for a single field un-comment the line above and remove the one below
            enrichmentModel.CourseLength = (courseLength == "Other" && string.IsNullOrEmpty(CourseLengthOther)) ? null : courseLength;//setting to null will trigger a validation

            enrichmentModel.SalaryDetails = SalaryDetails;
        }

        public IEnumerable<CopiedField> CopyFrom(CourseEnrichmentModel model)
        {
            var res = new List<CopiedField>();

            if (model == null)
            {
                return res;
            }

            if(Enum.TryParse(model.CourseLength, out CourseLength courseLength))
            {
                CourseLength = courseLength;
                res.Add(new CopiedField(nameof(model.CourseLength), CourseEnrichmentFieldNames.CourseLength));
            }
            //if (!CourseLength.HasValue && !string.IsNullOrEmpty(model.CourseLength))
            //{
            //    CourseLengthOther = model.CourseLength;
            //}
            //if we want a single field then replace this line with the one above
            if (!string.IsNullOrEmpty(model.CourseLengthOther))
            {
                CourseLengthOther = model.CourseLengthOther;
            }
            if (!string.IsNullOrEmpty(model.SalaryDetails))
            {
                SalaryDetails = model.SalaryDetails;
                res.Add(new CopiedField(nameof(model.SalaryDetails), CourseEnrichmentFieldNames.SalaryDetails));
            }
            
            return res;            
        }
    }
}
