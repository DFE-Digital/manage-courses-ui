using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseListViewModel
    {
        public string InstitutionName { get; set; }
        public string InstitutionId { get; set; }

        public List<Provider> Providers { get; set; }

        public string Status { get; set; }

        public bool MultipleOrganisations { get; set; }
    }
}
