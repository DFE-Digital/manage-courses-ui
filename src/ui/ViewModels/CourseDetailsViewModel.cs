using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseDetailsViewModel
    {
        public string CourseTitle { get; set; }
        public string CourseCode { get; set; }
        public string Type { get; set; }
        public string ProviderCode { get; set; }
        public string AccreditingProviderCode { get; set; }
        public string AccreditingProviderName { get; set; }
        public string Route { get; set; }
        public string Subjects { get; set; }
        public string AgeRange { get; set; }
        public string Qualifications { get; set; }
        public string StudyMode { get; set; }
        public string Regions { get; set; }
        public string Status { get; set; }

        public CourseRunningStatus StatusAsEnum =>
                string.Equals("running", Status ?? "", StringComparison.InvariantCultureIgnoreCase) ? CourseRunningStatus.Running
              : string.Equals("not running", Status ?? "", StringComparison.InvariantCultureIgnoreCase) ? CourseRunningStatus.NotRunning
              : CourseRunningStatus.New;

        public IEnumerable<SiteViewModel> Sites { get; set; }

    }
}
