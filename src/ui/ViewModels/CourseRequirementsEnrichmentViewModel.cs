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
            if (model == null)
            {
                yield break;
            }

            if (!string.IsNullOrEmpty(model.Qualifications))
            {
                Qualifications = model.Qualifications;
                yield return CourseEnrichmentFieldNames.Qualifications;
            }

            if (!string.IsNullOrEmpty(model.PersonalQualities))
            {
                PersonalQualities = model.PersonalQualities;
                yield return CourseEnrichmentFieldNames.PersonalQualities;
            }

            if (!string.IsNullOrEmpty(model.OtherRequirements))
            {
                OtherRequirements = model.OtherRequirements;
                yield return CourseEnrichmentFieldNames.OtherRequirements;
            }
        }
    }
}
