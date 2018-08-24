using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
    }
}
