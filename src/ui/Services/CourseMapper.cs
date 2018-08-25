using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.SearchAndCompare.Domain.Models;
using GovUk.Education.SearchAndCompare.Domain.Models.Enums;
using GovUk.Education.SearchAndCompare.Domain.Models.Joins;

namespace GovUk.Education.ManageCourses.Ui.Services
{
    public class CourseMapper : ICourseMapper
    {
        public SearchAndCompare.Domain.Models.Course MapToSearchAndCompareCourse(ApiClient.UcasInstitution ucasInstData, ApiClient.Course ucasCourseData, InstitutionEnrichmentModel orgEnrichmentModel, CourseEnrichmentModel courseEnrichmentModel)
        {
            orgEnrichmentModel = orgEnrichmentModel ?? new InstitutionEnrichmentModel();
            courseEnrichmentModel = courseEnrichmentModel ?? new CourseEnrichmentModel();

            var provider = new SearchAndCompare.Domain.Models.Provider
            {
                Name = ucasInstData.InstFull,
                ProviderCode = ucasInstData.InstCode
            };

            var accreditingProvider = ucasCourseData.AccreditingProviderId == null ? null :
                new SearchAndCompare.Domain.Models.Provider
                {
                    Name = ucasCourseData.AccreditingProviderName,
                    ProviderCode = ucasCourseData.AccreditingProviderId
                };

            // todo refactor out from Extension Method
            var routeName = new CourseVariantViewModel { Route = ucasCourseData.ProgramType }.GetRoute();
            var isSalaried = routeName.IndexOf("salaried") > -1;

            var mappedCourse = new SearchAndCompare.Domain.Models.Course
            {
                Duration = courseEnrichmentModel.CourseLength,
                Name = ucasCourseData.Name,
                ProgrammeCode = ucasCourseData.CourseCode,
                Provider = provider,                
                ProviderCodeName = ucasInstData.InstBig, // ???   
                AccreditingProvider = accreditingProvider,
                Route = new Route
                {
                    Name = routeName,
                    IsSalaried = isSalaried
                }, 
                IncludesPgce = string.IsNullOrWhiteSpace(ucasCourseData.ProfpostFlag) ? IncludesPgce.Yes : IncludesPgce.No,
                Campuses = new Collection<SearchAndCompare.Domain.Models.Campus>(ucasCourseData.Schools.Select(school => 
                    new SearchAndCompare.Domain.Models.Campus
                    {
                        Name = school.LocationName,
                        CampusCode = school.Code,
                        Location = new Location
                        {
                            Address = MapAddress(school),

                            // todo: still relevant?
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
                    Uk = (int) courseEnrichmentModel.FeeUkEu,
                    Eu = (int) courseEnrichmentModel.FeeUkEu,
                    International = (int) courseEnrichmentModel.FeeInternational,
                },
                
                IsSalaried = isSalaried,                

                ContactDetails = new Contact
                {
                    Phone = null, // ???
                    Fax = null, // ???
                    Email = null, // ???
                    Website = ucasInstData.Url,
                    Address = MapAddress(ucasInstData)
                },

                ApplicationsAcceptedFrom = ucasCourseData.Schools.Select(x => {
                        DateTime parsed;                    
                        return DateTime.TryParse(x.ApplicationsAcceptedFrom, out parsed) ? (DateTime?) parsed : null;
                    }).Where(x => x != null)
                    .OrderBy(x => x.Value)
                    .FirstOrDefault(),

                FullTime = ucasCourseData.StudyMode == "P" ? VacancyStatus.NA : VacancyStatus.Vacancies,
                PartTime = ucasCourseData.StudyMode == "F" ? VacancyStatus.NA : VacancyStatus.Vacancies, 
                                
                // todo update CourseEnrichmentModel
                Salary = new Salary
                {
                    // ???
                },

                // no longer needed?
                // todo refine Domain Model to include Mid Range
                //AgeRange = ucasCourseData.AgeRange.Trim().ToLowerInvariant() == "p" ? AgeRange.Primary 
                //    : ucasCourseData.AgeRange.Trim().ToLowerInvariant() == "m" ? AgeRange.MiddleYears
                //    : AgeRange.Secondary, 
                
                // no longer needed?
                //StartDate = null, // ???
            };

            mappedCourse.DescriptionSections = new Collection<CourseDescriptionSection>();

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "about this training programme",
                Text = courseEnrichmentModel.AboutCourse});

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "interview process",
                Text = courseEnrichmentModel.AboutCourse});

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "about fees",
                Text = courseEnrichmentModel.FeeDetails});

            var requirements = new List<string>();
            if (!string.IsNullOrWhiteSpace(courseEnrichmentModel.Qualifications)) 
            {
                requirements.Add("==Qualifications==\n\n" + courseEnrichmentModel.Qualifications);
            }
            if (!string.IsNullOrWhiteSpace(courseEnrichmentModel.PersonalQualities))
            {
                requirements.Add("==Personal qualities==\n\n" + courseEnrichmentModel.PersonalQualities);
            }            
            if (!string.IsNullOrWhiteSpace(courseEnrichmentModel.OtherRequirements))
            {
                requirements.Add("==Other requirements==\n\n" + courseEnrichmentModel.OtherRequirements);
            }

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "entry requirements",
                Text = String.Join("\n\n", requirements)
            });

            
            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "about school placements",
                Text = courseEnrichmentModel.HowSchoolPlacementsWork
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection{
                Name = "about this training provider",
                Text = String.Join("\n\n", new List<string> {
                    orgEnrichmentModel.TrainWithUs,
                    GetAccreditingProviderEnrichment(ucasCourseData.AccreditingProviderId, orgEnrichmentModel)
                })
            });

            mappedCourse.DescriptionSections.Add(new CourseDescriptionSection {
                Name = "training with disabilities",
                Text = orgEnrichmentModel.TrainWithDisability
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

            return "==About the accredited provider==\n\n" + enrichment.Description;
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

        private string MapAddress(UcasInstitution inst)
        {
            var addressFragments =  new List<string>{
                inst.Addr1,
                inst.Addr2,
                inst.Addr3,
                inst.Addr4
            }.Where(x => !string.IsNullOrWhiteSpace(x));

            var postCode = inst.Postcode ?? "";

            return addressFragments.Any()
                ? String.Join("\n", addressFragments) + "\n" + postCode
                : postCode;
        }

    }
}