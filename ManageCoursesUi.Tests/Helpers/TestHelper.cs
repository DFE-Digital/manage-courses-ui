using System.Collections.ObjectModel;
using GovUk.Education.ManageCourses.ApiClient;

namespace ManageCoursesUi.Tests.Helpers
{
    internal enum EnumDataType
    {
        HappyPath,
        ExceptionPath
    }
    internal static class TestHelper
    {
        public static string CourseTitle { get; } = "Chemistry";
        public static string OrganisationId { get; } = "5697";
        public static string OrganisationName { get; } = "Test Organisation";
        public static string InstitutionCode { get; } = "2AT";
        public static string AccreditedProviderId { get; } = "self";
        public static string UcasCode { get; } = "35L6";

        public static OrganisationCourses GetTestData(EnumDataType dataType)
        {
            OrganisationCourses data = null;
            switch (dataType)
            {
                case EnumDataType.HappyPath:
                    data = GetHappyPathData();
                    break;
                case EnumDataType.ExceptionPath:
                    data = GetExceptionPathData();
                    break;
            }

            return data;
        }

        private static OrganisationCourses GetHappyPathData()
        {
            var testData = new OrganisationCourses
            {
                OrganisationId = OrganisationId,
                OrganisationName = OrganisationName,
                UcasCode = UcasCode,
                ProviderCourses = new ObservableCollection<ProviderCourse>
                {
                    new ProviderCourse
                    {
                        CourseDetails = new ObservableCollection<CourseDetail>
                        {
                            new CourseDetail
                            {
                                AgeRange = "S",
                                CourseTitle = CourseTitle,
                                Variants = new ObservableCollection<CourseVariant>
                                {
                                    new CourseVariant
                                    {
                                        StudyMode = "F",
                                        ProfPostFlag = "",
                                        ProgramType = "SC",
                                        Subjects = new ObservableCollection<string>
                                        {
                                            "Chemistry",
                                            "Secondary",
                                            "Science"
                                        },
                                        CourseCode = "001",
                                        Name = CourseTitle,
                                        UcasCode = "234",
                                        Campuses = new ObservableCollection<Campus>()
                                    },
                                    new CourseVariant
                                    {
                                        StudyMode = "F",
                                        ProfPostFlag = "",
                                        ProgramType = "SC",
                                        Subjects = new ObservableCollection<string>
                                        {
                                            "Chemistry",
                                            "Secondary",
                                            "Science"
                                        },
                                        CourseCode = UcasCode,
                                        Name = CourseTitle,
                                        UcasCode = UcasCode,
                                        Campuses = new ObservableCollection<Campus>
                                        {
                                            new Campus{CourseOpenDate = "2018-10-16 00:00:00", Code = UcasCode, Name = CourseTitle, Address1 = "address1", Address2 = "address2", Address3 = "address3", Address4 = "address4", PostCode = "AS123D"}
                                        }
                                    },
                                    new CourseVariant
                                    {
                                        StudyMode = "F",
                                        ProfPostFlag = "",
                                        ProgramType = "SC",
                                        Subjects = new ObservableCollection<string>
                                        {
                                            "Chemistry",
                                            "Secondary",
                                            "Science"
                                        },
                                        CourseCode = "002",
                                        Name = CourseTitle,
                                        UcasCode = "890",
                                        Campuses = new ObservableCollection<Campus>()
                                    }
                                }
                            },
                        }
                    }
                }
            };

            return testData;
        }
        private static OrganisationCourses GetExceptionPathData()
        {
            var testData = new OrganisationCourses
            {
                OrganisationId = OrganisationId,
                OrganisationName = OrganisationName,
                UcasCode = UcasCode,
                ProviderCourses = new ObservableCollection<ProviderCourse>
                {
                    new ProviderCourse
                    {
                        CourseDetails = new ObservableCollection<CourseDetail>
                        {
                            new CourseDetail
                            {
                                AgeRange = "S",
                                CourseTitle = CourseTitle,
                                Variants = new ObservableCollection<CourseVariant>
                                {
                                    new CourseVariant
                                    {
                                        StudyMode = "F",
                                        ProfPostFlag = "",
                                        ProgramType = "SC",
                                        Subjects = new ObservableCollection<string>
                                        {
                                            "Chemistry",
                                            "Secondary",
                                            "Science"
                                        },
                                        CourseCode = "001",
                                        Name = CourseTitle,
                                        UcasCode = "234",
                                        Campuses = new ObservableCollection<Campus>()
                                    },
                                    new CourseVariant
                                    {
                                        StudyMode = "F",
                                        ProfPostFlag = "",
                                        ProgramType = "SC",
                                        Subjects = new ObservableCollection<string>
                                        {
                                            "Chemistry",
                                            "Secondary",
                                            "Science"
                                        },
                                        CourseCode = "002",
                                        Name = CourseTitle,
                                        UcasCode = "890",
                                        Campuses = new ObservableCollection<Campus>()
                                    }
                                }
                            },
                        }
                    }
                }
            };

            return testData;
        }
    }
}
