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

        public bool IsEmpty() =>
            string.IsNullOrEmpty(Qualifications) &&
            string.IsNullOrEmpty(PersonalQualities) &&
            string.IsNullOrEmpty(OtherRequirements);

        public void MapInto(ref CourseEnrichmentModel enrichmentModel)
        {        
            enrichmentModel.Qualifications = Qualifications;
            enrichmentModel.PersonalQualities = PersonalQualities;
            enrichmentModel.OtherRequirements = OtherRequirements;
        }

        public IEnumerable<CopiedField> CopyFrom(CourseEnrichmentModel model)
        {
            var res = new List<CopiedField>();
            if (model == null)
            {
                return res;
            }

            if (!string.IsNullOrEmpty(model.Qualifications))
            {
                Qualifications = model.Qualifications;
                res.Add(new CopiedField(nameof(model.Qualifications), CourseEnrichmentFieldNames.Qualifications));
            }

            if (!string.IsNullOrEmpty(model.PersonalQualities))
            {
                PersonalQualities = model.PersonalQualities;
                res.Add(new CopiedField(nameof(model.PersonalQualities), CourseEnrichmentFieldNames.PersonalQualities));
            }

            if (!string.IsNullOrEmpty(model.OtherRequirements))
            {
                OtherRequirements = model.OtherRequirements;
                res.Add(new CopiedField(nameof(model.OtherRequirements), CourseEnrichmentFieldNames.OtherRequirements));
            }

            return res;
        }
    }
}
