using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class Provider
    {
        public Provider()
        {
            Courses = new List<Course>();
        }
        public string ProviderName { get; set; }
        public string ProviderId { get; set; }
        public List<Course> Courses { get; set; }
        public int TotalCount { get; set; }
    }
}
