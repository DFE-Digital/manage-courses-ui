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
        public static string GetCourseStatus(this Course course)
        {
            var result = "";
            if (course.Schools.Any(s => String.Equals(s.Status, "r", StringComparison.InvariantCultureIgnoreCase) && String.Equals(s.Publish, "y", StringComparison.InvariantCultureIgnoreCase)))
            {
                result = "Running";
            }
            else {
                if (course.Schools.Any(s => String.Equals(s.Status, "r", StringComparison.InvariantCultureIgnoreCase) && String.Equals(s.Publish, "n", StringComparison.InvariantCultureIgnoreCase))){
                    result  = "Running but incomplete";
                }
                else if (course.Schools.Any(s => String.Equals(s.Status, "n", StringComparison.InvariantCultureIgnoreCase)))
                {
                    result = "New – not yet running";
                }
                else if (course.Schools.Any(s => String.Equals(s.Status, "d", StringComparison.InvariantCultureIgnoreCase)) || course.Schools.Any(s => String.Equals(s.Status, "s", StringComparison.InvariantCultureIgnoreCase)))
                {
                    result = "Not running";
                }
            }

            return result;
        }

        public static bool CanHaveEnrichment(this Course course)
        {
            return course != null && course.GetCourseStatus() != "Not running";
        }

        public static string GetSiteStatus(this SiteViewModel school)
        {
            var result = "";
            switch ((school.Status ?? "").ToLower())
            {
                case "d":
                    result = "Discontinued";
                    break;
                case "r":
                    {
                        result = (school.Publish ?? "").Equals("y", StringComparison.InvariantCultureIgnoreCase) ? "Running" : "Running but incomplete";

                        break;
                    }
                case "n":
                    result = "New";
                    break;
                case "s":
                    result = "Suspended";
                    break;
            }
            return result;
        }

        public static string GetVacancyStatus(this SiteViewModel school)
        {
            var result = "";
            switch (school.VacStatus?.ToLower())
            {
                case "b":
                    result = "Both full time and part time vacancies";
                    break;
                case "p":
                    result = "Part time vacancies";
                    break;
                case "f":
                    result = "Full time vacancies";
                    break;
                default:
                    result = "No vacancies";
                    break;
            }
            return result;
        }

        public static string GetHasVacancies(this Course course)
        {
            return CourseIsDiscontinuedOrSuspended(course)
                ? ""
                : course.HasVacancies ? "Yes" : "No";
        }

        public static string GetRoute(this CourseDetailsViewModel viewModel)
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


        public static string GetStudyMode(this CourseDetailsViewModel viewModel)
        {
            return UppercaseFirst(GetStudyModeText(viewModel.StudyMode));
        }

        public static string GetAgeRange(this CourseDetailsViewModel viewModel)
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

        public static string GetQualification(this CourseDetailsViewModel viewModel)
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
        private static string GetStudyModeText(string studyMode)
        {
            var returnString = string.Empty;

            if (string.IsNullOrWhiteSpace(studyMode))
            {
                return returnString;//TODO clarify what happens if study mode is missing
            }

            if (studyMode.Equals("F", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "full time";
            }
            else if (studyMode.Equals("P", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "part time";
            }
            else if (studyMode.Equals("B", StringComparison.InvariantCultureIgnoreCase))
            {
                returnString = "full time or part time";
            }

            return returnString;
        }
        private static string UppercaseFirst(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }

        private static bool CourseIsDiscontinuedOrSuspended(this Course course)
        {
            return course.CourseSites.All(s => SiteIsDiscontinued(s) || SiteIsSuspended(s));
        }

        private static bool SiteIsDiscontinued(CourseSite courseSite)
        {
            return String.Equals(courseSite.Status, "d", StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool SiteIsSuspended(CourseSite courseSite)
        {
            return String.Equals(courseSite.Status, "s", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
