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
        public static string GetCourseVariantType(this Course course)
        {
            var result = string.IsNullOrWhiteSpace(course.ProfpostFlag) ? "QTS " : "PGCE with QTS ";

            result += course.StudyMode.ToLower().Equals("f", StringComparison.InvariantCultureIgnoreCase) ? "full time" : "part time";
            result += course.ProgramType.ToLower().Equals("ss", StringComparison.InvariantCultureIgnoreCase) ? " with salary" : "";

            return result;
        }
        public static string GetCourseStatus(this Course course)
        {
            var result = "";
            if (course.Schools.Any(s => s.Status.ToLower() == "r"))
            {
                result = "Running";
            }
            else if (course.Schools.Any(s => s.Status.ToLower() == "n"))
            {
                result = "New – not yet running";
            }
            else if (course.Schools.Any(s => s.Status.ToLower() == "d") || course.Schools.Any(s => s.Status.ToLower() == "s"))
            {
                result = "Not running";
            }
            return result;
        }
        public static string GetSchoolStatus(this SchoolViewModel school)
        {
            var result = "";
            switch (school.Status.ToLower())
            {
                case "d":
                    result = "Discontinued";
                    break;
                case "r":
                    result = "Running";
                    break;
                case "n":
                    result = "New";
                    break;
                case "s":
                    result = "Suspended";
                    break;
            }
            return result;
        }
        public static string GetRoute(this CourseVariantViewModel viewModel)
        {
            var result = "";

            var route = viewModel.Route.ToLowerInvariant();

            switch (route)
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
            var result = "";

            if (string.IsNullOrEmpty(viewModel.AgeRange)) return result;

            var ageRange = viewModel.AgeRange.Trim().ToLowerInvariant();
            
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

        public static string GetQualification(this CourseVariantViewModel viewModel)
        {
            var result = "";

            if (viewModel.Qualifications == null) return result;

            var qualifications = viewModel.Qualifications.Trim().ToLowerInvariant();

            switch (qualifications)
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
                case "":
                {
                    result = "Recommendation for QTS";
                        break;
                }
                default:
                {
                    break;
                }
            }

            return result;
        }
    }
}

