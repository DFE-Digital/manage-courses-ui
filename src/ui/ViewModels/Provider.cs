using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class Provider
    {
        public Provider()
        {
            Courses = new List<Course>();
        }
        public string InstName { get; set; }
        public string InstCode { get; set; }
        public List<Course> Courses { get; set; }
        public int TotalCount { get; set; }
    }
}
