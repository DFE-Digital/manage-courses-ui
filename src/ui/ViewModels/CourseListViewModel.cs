using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseListViewModel
    {
        public OrganisationCourses Courses { get; set; }
        public int TotalCount { get; set; }
    }
}
