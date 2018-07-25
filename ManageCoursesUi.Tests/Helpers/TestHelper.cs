using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GovUk.Education.ManageCourses.ApiClient;
using ManageCoursesUi.Tests.Enums;

namespace ManageCoursesUi.Tests.Helpers
{
    /// <summary>
    /// This helper class generates the exact structure of the data returned from the real Api.
    /// It also generates non-targeted data so that the end point will error
    /// </summary>
    internal static class TestHelper
    {        
        public static string OrganisationId { get; } = "5697";
        public static string OrganisationName { get; } = "Test Organisation";
        public static string InstitutionCode { get; } = "2AT";
        public static string AccreditedProviderId { get; } = "self";
        public static string TargetedUcasCode { get; } = "35L6";
        public static string TargetedCourseTitle { get; } = "Chemistry";

        private static string _courseTitles = "Maths,Chemistry,Biology,Music,Languages";

        public static OrganisationCourses GetTestData(EnumDataType dataType, string accreditedProviderId, string accreditedProviderName)
        {
            return GenerateData(dataType, accreditedProviderId, accreditedProviderName);
        }

        /// <summary>
        /// Creates the top level OrganisationCourse class
        /// Call the method to create the course details
        /// </summary>
        /// <param name="dataType">Defines the 2 types of data setup that is expected from the Api plus one that is not expected</param>
        /// <param name="accreditedProviderId">used to setup the providerCourse object</param>
        /// <param name="accreditedProviderName">used to setup the providerCourse object</param>
        /// <returns></returns>
        private static OrganisationCourses GenerateData(EnumDataType dataType, string accreditedProviderId, string accreditedProviderName)
        {
            var testData = new OrganisationCourses
            {
                OrganisationId = OrganisationId,
                OrganisationName = OrganisationName,
                UcasCode = TargetedUcasCode,
                ProviderCourses = new ObservableCollection<ProviderCourse>
                {
                    new ProviderCourse
                    {
                        AccreditingProviderId = accreditedProviderId,
                        AccreditingProviderName = accreditedProviderName,
                        CourseDetails = new ObservableCollection<CourseDetail>(GenerateCourseDetails(dataType))
                    }
                }
            };

            return testData;
        }
        /// <summary>
        /// Creates the course details list
        /// </summary>
        /// <param name="type">The type of test data that needs to be generated</param>
        /// <returns></returns>
        private static List<CourseDetail> GenerateCourseDetails(EnumDataType type)
        {
            var listToReturn = new List<CourseDetail>();            
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
                listToReturn.Add(
                    new CourseDetail
                    {
                        AgeRange = "S",
                        CourseTitle = courseTitle,
                        Variants = new ObservableCollection<CourseVariant>(GenerateCourseVariants(variantCount, (courseTitle == TargetedCourseTitle && happyPath), courseTitle))
                    }
                );
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
        private static List<CourseVariant> GenerateCourseVariants(int count, bool happyPath, string courseTitle)
        {
            var listToReturn = new List<CourseVariant>();

            for (var counter = 1; counter <= count; counter++)
            {                
                var targetedPosition = count > 1 ? 2 : 1;//sets the position of the targed variant. This is important as there have been errors when the targed variant is not in the first position
                var ucasCode = "00";
                if (happyPath && counter == targetedPosition)
                {
                    ucasCode = TargetedUcasCode;//this code should be present for a happy path test
                }
                else
                {
                    ucasCode += counter;
                }
                listToReturn.Add(
                    new CourseVariant
                    {
                        StudyMode = "F",
                        ProfPostFlag = "",
                        ProgramType = "SC",
                        Subjects = new ObservableCollection<string>
                        {
                            courseTitle,
                            "Secondary"
                        },
                        CourseCode = ucasCode,
                        Name = courseTitle,
                        UcasCode = ucasCode,
                        Campuses = new ObservableCollection<Campus>
                        {
                            new Campus
                            {
                                CourseOpenDate = "2018-10-16 00:00:00",
                                Code = TargetedUcasCode,
                                Name = courseTitle,
                                Address1 = "address1",
                                Address2 = "address2",
                                Address3 = "address3",
                                Address4 = "address4",
                                PostCode = "AS123D"
                            }
                        }
                    }
                );
            }
            return listToReturn;
        }
    }
}
