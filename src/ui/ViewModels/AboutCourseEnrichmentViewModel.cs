using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class AboutCourseEnrichmentViewModel : ICourseEnrichmentViewModel
    {

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,400}$", ErrorMessage = "Reduce the word count for about this course")]
        public string AboutCourse { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for interview process")]
        public string InterviewProcess { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,350}$", ErrorMessage = "Reduce the word count for how school placements work")]
        public string HowSchoolPlacementsWork { get; set; }

        public CourseRouteDataViewModel RouteData { get; set; }

        public CourseInfoViewModel CourseInfo { get; set; }

        public bool IsEmpty() =>
            string.IsNullOrEmpty(AboutCourse) &&
            string.IsNullOrEmpty(InterviewProcess) &&
            string.IsNullOrEmpty(HowSchoolPlacementsWork);

        public void MapInto(ref CourseEnrichmentModel enrichmentModel)
        {
            enrichmentModel.AboutCourse = AboutCourse;
            enrichmentModel.InterviewProcess = InterviewProcess;
            enrichmentModel.HowSchoolPlacementsWork = HowSchoolPlacementsWork;
        }

        public IEnumerable<CopiedField> CopyFrom(CourseEnrichmentModel model)
        {
            var res = new List<CopiedField>();

            if (model == null)
            {
                return res;
            }

            if (!string.IsNullOrWhiteSpace(model.AboutCourse))
            {
                AboutCourse = model.AboutCourse;
                res.Add(new CopiedField(nameof(model.AboutCourse), CourseEnrichmentFieldNames.AboutCourse));
            }

            if (!string.IsNullOrWhiteSpace(model.InterviewProcess))
            {
                InterviewProcess = model.InterviewProcess;
                res.Add(new CopiedField(nameof(model.InterviewProcess), CourseEnrichmentFieldNames.InterviewProcess));
            }

            if (!string.IsNullOrWhiteSpace(model.HowSchoolPlacementsWork))
            {
                HowSchoolPlacementsWork = model.HowSchoolPlacementsWork;
                res.Add(new CopiedField(nameof(model.HowSchoolPlacementsWork), CourseEnrichmentFieldNames.HowSchoolPlacementsWork));
            }

            return res;
        }
    }
}
