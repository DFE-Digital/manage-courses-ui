using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseListViewModel
    {
        public string ProviderName { get; set; }
        public string ProviderCode { get; set; }
        public bool ProviderOptedIn { get; set; }

        public List<Provider> Providers { get; set; }

        public string Status { get; set; }

        public bool MultipleOrganisations { get; set; }
    }
}
