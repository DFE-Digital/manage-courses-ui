using System.Collections.Generic;


using System.ComponentModel.DataAnnotations;
namespace GovUk.Education.ManageCourses.Ui.ViewModels
{

    public class AboutCourseEnrichmentViewModel
    {

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,400}$", ErrorMessage = "Reduce the word count for details about this course")]

        public string AboutCourse { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for details about interview process")]
                public string InterviewProcess { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,350}$", ErrorMessage = "Reduce the word count for details how school placements work")]

        public string HowSchoolPlacementsWork { get; set; }

        public CourseRouteDataViewModel RouteData { get; set; }
    }
}
