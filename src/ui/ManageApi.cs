using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels;

namespace GovUk.Education.ManageCourses.Ui
{
    public class ManageApi : IManageApi
    {
        private readonly ManageCoursesApiClient _apiClient;

        public ManageApi(ManageCoursesApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // Do not handled any exception let it thro as it should be handled by McExceptionFilter or startup configuration.
        public async Task<UserOrganisation> GetOrganisation(string instCode)
        {
            var courses = await _apiClient.Organisations_GetAsync(instCode);
            return courses;
        }

        public async Task<UcasInstitution> GetUcasInstitution(string instCode)
        {
            var inst = await _apiClient.Organisations_GetUcasInstitutionAsync(instCode);
            return inst;
        }

        public async Task<InstitutionCourses> GetCoursesByOrganisation(string instCode)
        {
            var courses = await _apiClient.Courses_GetAllAsync(instCode);
            return courses;
        }
        public async Task<Course> GetCourseByUcasCode(string instCode, string ucasCode)
        {
            var course = await _apiClient.Courses_GetAsync(instCode, ucasCode);
            return course;
        }

        public async Task<IEnumerable<UserOrganisation>> GetOrganisations()
        {
            var orgs = await _apiClient.Organisations_GetAllAsync();
            return orgs;
        }

        public async Task LogAccessRequest(AccessRequest accessRequest)
        {
            await _apiClient.AccessRequest_IndexAsync(accessRequest);
        }

        public async Task LogAcceptTerms()
        {
            await _apiClient.AcceptTerms_IndexAsync();
        }

        public async Task SaveEnrichmentOrganisation(string institutionCode, UcasInstitutionEnrichmentPostModel organisation)
        {
            await _apiClient.Enrichment_SaveInstitutionAsync(institutionCode, organisation);
        }

        public async Task<UcasInstitutionEnrichmentGetModel> GetEnrichmentOrganisation(string ucasCode)
        {
            var result = await _apiClient.Enrichment_GetInstitutionAsync(ucasCode);

            result = result ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            return result;
        }

        public async Task<bool> PublishEnrichmentOrganisation(string ucasCode)
        {
            var result = await _apiClient.Enrichment_PublishInstitutionAsync(ucasCode);

            return result;
        }

        public async Task<UcasCourseEnrichmentGetModel> GetEnrichmentCourse(string instCode, string ucasCode)
        {
            var result = await _apiClient.Enrichment_GetCourseAsync(instCode, ucasCode);

            return result;
        }
        public async Task SaveEnrichmentCourse(string instCode, string ucasCode, CourseEnrichmentModel course)
        {
            await _apiClient.Enrichment_SaveCourseAsync(instCode, ucasCode, course);
        }
        
        public async Task<bool> PublishEnrichmentCourse(string ucasCode, string courseCode)
        {
            await _apiClient.Enrichment_PublishCourseAsync(ucasCode, courseCode);

            return true;
        }

        public async Task<UcasCourseEnrichmentGetModel> GetEnrichmentCourse(string instCode, string ucasCode)
        {
            var result = await _apiClient.Enrichment_GetCourseAsync(instCode, ucasCode);

            return result;
        }
        public async Task SaveEnrichmentCourse(string instCode, string ucasCode, CourseEnrichmentModel course)
        {
            await _apiClient.Enrichment_SaveCourseAsync(instCode, ucasCode, course);
        }
    }
}
