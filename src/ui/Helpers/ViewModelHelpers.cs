using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;

namespace GovUk.Education.ManageCourses.Ui.Helpers
{
    public static class ViewModelHelpers
    {
        public static string GetCourseVariantType(this CourseVariant courseVariant)
        {
            var result = string.IsNullOrWhiteSpace(courseVariant.ProfPostFlag) ? "QTS " : "PGCE with QTS ";

            result += courseVariant.StudyMode.Equals("F", StringComparison.InvariantCultureIgnoreCase) ? "full time" : "part time";
            result += courseVariant.ProgramType.Equals("SS", StringComparison.InvariantCultureIgnoreCase) ? " with salary" : "";

            return result;
        }



        public static string GetQualification(this CourseVariantViewModel viewModel)
        {
            var result = "";

            var qualifications = viewModel.Qualifications.ToLowerInvariant();

            switch (qualifications)
            {
                case "he":
                    {
                        result = "Higher education programme";
                        break;
                    }
                case "sd":
                    {
                        result = "School Direct training programme";
                        break;
                    }
                case "ss":
                    {
                        result = "School Direct (salaried) training programme";
                        break;
                    }
                case "sc":
                    {
                        result = "SCITT programme";
                        break;
                    }
                case "ta":
                    {
                        result = "PG Teaching Apprenticeship";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return result;
        }


        public static string GetStudyMode(this CourseVariantViewModel viewModel)
        {
            return viewModel.StudyMode.Equals("F", StringComparison.InvariantCultureIgnoreCase) ? "Full time" : "Part time";
        }

        public static string GetAgeRange(this CourseVariantViewModel viewModel)
        {
            var ageRange = viewModel.AgeRange.ToLowerInvariant();

            var result = "";
            switch (ageRange)
            {
                case "s":
                    {
                        result = "Secondary (11+ years)";
                        break;
                    }
                case "m":
                    {
                        result = "Middle years (7 - 14 years)";
                        break;
                    }
                case "p":
                    {
                        result = "Primary (3 - 11/12 years)";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return result;
        }

        public static string GetRoute(this CourseVariantViewModel viewModel)
        {
            var result = "";

            var route = viewModel.Route.ToLowerInvariant();

            switch (route)
            {
                case "pf":
                    {
                        result = "Professional";
                        break;
                    }
                case "pg":
                    {
                        result = "Postgraduate";
                        break;
                    }
                case "bo":
                    {
                        result = "Professional/Postgraduate";
                        break;
                    }
                default:
                    {
                        result = "Recommendation for QTS";
                        break;
                    }
            }

            return result;
        }
    }
}

