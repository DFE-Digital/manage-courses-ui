using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.SearchAndCompare.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using GovUk.Education.SearchAndCompare.Domain.Models.Joins;
using GovUk.Education.SearchAndCompare.UI.Shared.Services;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public class CourseDetailsService : ICourseDetailsService
    {
        private readonly IManageApi api;

        public CourseDetailsService(IManageApi api)
        {
            this.api = api;
        }

        public SearchAndCompare.Domain.Models.Course GetCourse(string providerCode, string courseCode)
        {
            var ucasCourseData = api.GetCourseByUcasCode(providerCode, courseCode).Result;
            var orgEnrichmentData = api.GetEnrichmentOrganisation(providerCode).Result;
            var courseEnrichmentData = api.GetEnrichmentCourse(providerCode, courseCode).Result;

            var provider = new Provider
            {
                Name = "", // ??!?!?
                ProviderCode = ucasCourseData.InstCode
            };

            var accreditingProvider = ucasCourseData.AccreditingProviderId == null ? null :
                new Provider 
                {
                    Name = ucasCourseData.AccreditingProviderName,
                    ProviderCode = ucasCourseData.AccreditingProviderId
                };

            var mappedCourse = new SearchAndCompare.Domain.Models.Course
            {
                Duration = courseEnrichmentData.EnrichmentModel.CourseLength,
                Name = ucasCourseData.Name,
                ProgrammeCode = ucasCourseData.CourseCode,
                ProviderCodeName = "", // ???
                Provider = provider,
                AccreditingProvider = accreditingProvider,
                AgeRange = AgeRange.Secondary, // ???
                Route = new Route(), // ???
                IncludesPgce = string.IsNullOrWhiteSpace(ucasCourseData.ProfpostFlag) ? IncludesPgce.Yes : IncludesPgce.No,
                Campuses = new Collection<SearchAndCompare.Domain.Models.Campus>(ucasCourseData.Schools.Select(school => 
                    new SearchAndCompare.Domain.Models.Campus
                    {
                        Name = school.LocationName,
                        CampusCode = school.Code,
                        Location = new Location
                        {
                            Address = MapAddress(school),
                            Latitude = 0,
                            Longitude = 0
                        }
                    }
                ).ToList()),
                CourseSubjects = new Collection<CourseSubject>(ucasCourseData.Subjects.Split(", ").Select(subject =>
                new CourseSubject
                {
                    Subject = new Subject
                    {
                        Name = subject
                    }

                }).ToList()),
                Fees = new Fees
                {
                    Uk = (int) courseEnrichmentData.EnrichmentModel.FeeUkEu,
                    Eu = (int) courseEnrichmentData.EnrichmentModel.FeeUkEu,
                    International = (int) courseEnrichmentData.EnrichmentModel.FeeInternational,
                }, // ???
                
                IsSalaried = false, // ???

                Salary = new Salary
                {
                    // ???
                },

                ContactDetails = new Contact
                {
                    Phone = null, // ???
                    Fax = null, // ???
                    Email = null, // ???
                    Website = null, // ???
                    Address = null // ???
                },

                FullTime = ucasCourseData.StudyMode == "P" ? VacancyStatus.NA : VacancyStatus.Vacancies,
                PartTime = ucasCourseData.StudyMode == "F" ? VacancyStatus.NA : VacancyStatus.Vacancies,

                ApplicationsAcceptedFrom = ucasCourseData.Schools.Select(x => {
                        DateTime parsed;                    
                        return DateTime.TryParse(x.ApplicationsAcceptedFrom, out parsed) ? (DateTime?) parsed : null;
                    }).Where(x => x != null)
                    .OrderBy(x => x.Value)
                    .FirstOrDefault(),


                StartDate = null // ???
            };

            mappedCourse.DescriptionSections = new Collection<CourseDescriptionSection>();

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "about this training programme",
                Text = String.Join("\n\n", new List<string> {
                    courseEnrichmentData.EnrichmentModel.AboutCourse,
                    courseEnrichmentData.EnrichmentModel.InterviewProcess,
                    courseEnrichmentData.EnrichmentModel.HowSchoolPlacementsWork
                })
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "entry requirements",
                Text = String.Join("\n\n", new List<string> {
                    courseEnrichmentData.EnrichmentModel.Qualifications,
                    courseEnrichmentData.EnrichmentModel.PersonalQualities,
                    courseEnrichmentData.EnrichmentModel.OtherRequirements,
                })
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "about this training provider",
                Text = String.Join("\n\n", new List<string> {
                    orgEnrichmentData.EnrichmentModel.TrainWithUs,
                    GetAccreditingProviderEnrichment(ucasCourseData.AccreditingProviderId, orgEnrichmentData.EnrichmentModel),
                    orgEnrichmentData.EnrichmentModel.TrainWithDisability
                })
            });

            return mappedCourse;
        }

        private string GetAccreditingProviderEnrichment(string accreditingProviderId, InstitutionEnrichmentModel enrichmentModel)
        {
            if (string.IsNullOrWhiteSpace(accreditingProviderId))
            {
                return "";
            }

            var enrichment = enrichmentModel.AccreditingProviderEnrichments.FirstOrDefault(x => x.UcasInstitutionCode == accreditingProviderId);

            if (enrichment == null)
            {
                return "";
            }

            return enrichment.Description;
        }

        private string MapAddress(School school)
        {
            var addressFragments =  new List<string>{
                school.Address1,
                school.Address2,
                school.Address3,
                school.Address4
            }.Where(x => !string.IsNullOrWhiteSpace(x));

            var postCode = school.PostCode ?? "";

            return addressFragments.Any()
                ? String.Join(", ", addressFragments) + " " + postCode
                : postCode;
        }

        public List<FeeCaps> GetFeeCaps()
        {
            return new List<FeeCaps> {new FeeCaps()};
        }
    }
}