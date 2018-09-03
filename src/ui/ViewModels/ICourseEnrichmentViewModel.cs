using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public interface ICourseEnrichmentViewModel 
    { 
        bool IsEmpty();
        void MapInto(ref CourseEnrichmentModel enrichmentModel);
        void CopyFrom(CourseEnrichmentModel model);
    }
    }
}
