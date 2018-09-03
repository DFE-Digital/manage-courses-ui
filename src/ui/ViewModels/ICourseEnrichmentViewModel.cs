using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public interface ICourseEnrichmentViewModel 
    {
        IEnumerable<string> CopyFrom(CourseEnrichmentModel model);
    }
}
