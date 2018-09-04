using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseRequirementsEnrichmentViewModel : ICourseEnrichmentViewModel
    {
        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for qualifications")]
        public string Qualifications { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for personal qualities")]
        public string PersonalQualities { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Reduce the word count for other requirements")]
        public string OtherRequirements { get; set; }

        public CourseRouteDataViewModel RouteData { get; set; }

        public CourseInfoViewModel CourseInfo { get; set; }

        public IEnumerable<string> CopyFrom(CourseEnrichmentModel model)
        {
            var res = new List<string>();
            if (model == null)
            {
                return res;
            }

            if (!string.IsNullOrEmpty(model.Qualifications))
            {
                Qualifications = model.Qualifications;
                res.Add(CourseEnrichmentFieldNames.Qualifications);
            }

            if (!string.IsNullOrEmpty(model.PersonalQualities))
            {
                PersonalQualities = model.PersonalQualities;
                res.Add(CourseEnrichmentFieldNames.PersonalQualities);
            }

            if (!string.IsNullOrEmpty(model.OtherRequirements))
            {
                OtherRequirements = model.OtherRequirements;
                res.Add(CourseEnrichmentFieldNames.OtherRequirements);
            }

            return res;
        }
    }
}
