using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class TabViewModel
    {
        public bool MultipleOrganisations { get; set; }
        public string OrganisationName { get; set; }
        public string CurrentTab { get; set; }
        public string UcasCode { get; set; }
    }
}
