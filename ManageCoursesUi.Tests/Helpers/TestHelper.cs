using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using ManageCoursesUi.Tests.Enums;

namespace ManageCoursesUi.Tests.Helpers
{
    /// <summary>
    /// This helper class generates the exact structure of the data returned from the real Api.
    /// It also generates non-targeted data so that the end point will error
    /// </summary>
    internal static class TestHelper
    {
        public static string ProviderName { get; } = "Test Organisation";
        public static string ProviderCode { get; } = "2AT";
        public static string AccreditingProviderCode { get; } = "self";
        public static string TargetedProviderCode { get; } = "35L6";
        public static string TargetedCourseTitle { get; } = "Chemistry";

        private static string _courseTitles = "Maths,Chemistry,Biology,Music,Languages";

        public static List<Course> GetTestData(EnumDataType dataType)
        {
            return GenerateCourseDetails(dataType);
        }
        /// <summary>
        /// Creates the course details list
        /// </summary>
        /// <param name="type">The type of test data that needs to be generated</param>
        /// <returns></returns>
        private static List<Course> GenerateCourseDetails(EnumDataType type)
        {
            var listToReturn = new List<Course>();
            int variantCount;
            bool happyPath;
            switch (type)
            {
                case EnumDataType.SingleVariantOneMatch:
                    happyPath = true;
                    variantCount = 1;
                    break;
                case EnumDataType.MultiVariantOneMatch:
                    happyPath = true;
                    variantCount = 3;
                    break;
                case EnumDataType.MultiVariantNoMatch:
                    happyPath = false;
                    variantCount = 1;
                    break;
                case EnumDataType.SingleVariantNoMatch:
                    happyPath = false;
                    variantCount = 3;
                    break;

                default:
                    happyPath = false;
                    variantCount = 1;
                    break;
            }

            foreach (var courseTitle in _courseTitles.Split(","))
            {
                listToReturn.AddRange(GenerateCourseVariants(variantCount, (courseTitle == TargetedCourseTitle && happyPath), courseTitle));
            }

            return listToReturn;
        }

        /// <summary>
        /// create the course variant list
        /// </summary>
        /// <param name="count">Number of variant to create</param>
        /// <param name="happyPath">If true it sets the Ucas code to the targeted ucas code for one variant</param>
        /// <param name="courseTitle">For the variant course name</param>
        /// <returns></returns>
        private static List<Course> GenerateCourseVariants(int count, bool happyPath, string courseTitle)
        {
            var listToReturn = new List<Course>();

            for (var counter = 1; counter <= count; counter++)
            {
                var targetedPosition = count > 1 ? 2 : 1;//sets the position of the targed variant. This is important as there have been errors when the targed variant is not in the first position
                var providerCode = "00";
                if (happyPath && counter == targetedPosition)
                {
                    providerCode = TargetedProviderCode;//this code should be present for a happy path test
                }
                else
                {
                    providerCode += counter;
                }
                listToReturn.Add(
                    new Course
                    {
                        StudyMode = "F",
                        ProfpostFlag = "",
                        ProgramType = "SC",
                        CourseSubjects = new List<CourseSubject>{ new CourseSubject{ Subject = new Subject {SubjectName = courseTitle + ",Secondary"}}},
                        CourseCode = "CC"+count,
                        Name = courseTitle,
                        Provider = new Provider { ProviderCode = providerCode },
                        CourseSites = new List<CourseSite>
                        {
                            new CourseSite {
                                Status = "R",
                                ApplicationsAcceptedFrom = DateTime.Parse("2018-10-16 00:00:00"),

                                Site = new Site
                                {
                                    Code = TargetedProviderCode,
                                    Address1 = "address1",
                                    Address2 = "address2",
                                    Address3 = "address3",
                                    Address4 = "address4",
                                    Postcode = "AS123D"
                            }}
                        }
                    }
                );
            }
            return listToReturn;
        }
    }
}
