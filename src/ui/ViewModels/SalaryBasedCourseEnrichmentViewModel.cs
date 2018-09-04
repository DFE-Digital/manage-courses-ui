using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{

    public class SalaryBasedCourseEnrichmentViewModel : BaseCourseEnrichmentViewModel
    {

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for salary")]
        [Required(ErrorMessage = "Give details about salary")]
        public string SalaryDetails { get; set; }

        public List<string> GetSalaryFields() => new List<string> { nameof(this.CourseLength), nameof(this.SalaryDetails) };
    }
}
