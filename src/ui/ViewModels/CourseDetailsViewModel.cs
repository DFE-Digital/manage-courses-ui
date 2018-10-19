using System;
using System.Linq;
using System.Collections.Generic;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseDetailsViewModel
    {
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public string Type { get; set; }
        public string InstCode { get; set; }
        public string AccreditingInstCode { get; set; }
        public string AccreditingInstName { get; set; }
        public string Route { get; set; }
        public string Subjects { get; set; }
        public string AgeRange { get; set; }
        public string Qualifications { get; set; }
        public string StudyMode { get; set; }
        public string Regions { get; set; }
        public string Status { get; set; }

        public CourseRunningStatus StatusAsEnum =>
            string.Equals("running", Status ?? "", StringComparison.InvariantCultureIgnoreCase) ?
                    (Schools.Any(x => x.Publish.Equals("n", StringComparison.InvariantCultureIgnoreCase) && x.Status.Equals("r", StringComparison.InvariantCultureIgnoreCase)) ?
                    CourseRunningStatus.RunningButNeedsAttention : CourseRunningStatus.Running)
              : string.Equals("not running", Status ?? "", StringComparison.InvariantCultureIgnoreCase) ? CourseRunningStatus.NotRunning
              : string.Equals("Needs attention in UCAS", Status ?? "", StringComparison.InvariantCultureIgnoreCase) ? CourseRunningStatus.NeedsAttention
              : CourseRunningStatus.New;

        public IEnumerable<SiteViewModel> Schools { get; set; }
    }
}
