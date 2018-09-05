using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public interface ICourseEnrichmentViewModel 
    { 
        bool IsEmpty();
        IEnumerable<CopiedField> CopyFrom(CourseEnrichmentModel model);
        void MapInto(ref CourseEnrichmentModel enrichmentModel);
    }
}
