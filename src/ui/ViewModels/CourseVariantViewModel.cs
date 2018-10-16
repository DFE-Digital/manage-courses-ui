using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseVariantViewModel
    {
        public string Name { get; set; }
        public string ProgrammeCode { get; set; }
        public string Type { get; set; }
        public string UcasCode { get; set; }
        public string ProviderCode { get; set; }
        public string Accrediting { get; set; }
        public string Route { get; set; }
        public string Subjects { get; set; }
        public string AgeRange { get; set; }
        public string Qualifications { get; set; }
        public string StudyMode { get; set; }
        public string Regions { get; set; }
        public string Status { get; set; }

        public CourseVariantStatus StatusAsEnum =>
                string.Equals("running", Status ?? "", StringComparison.InvariantCultureIgnoreCase) ? CourseVariantStatus.Running
              : string.Equals("not running", Status ?? "", StringComparison.InvariantCultureIgnoreCase) ? CourseVariantStatus.NotRunning
              : CourseVariantStatus.New;

        public IEnumerable<SchoolViewModel> Schools { get; set; }

    }
}
