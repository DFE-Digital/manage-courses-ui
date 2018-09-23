using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class OrganisationViewModel
    {
        public OrganisationViewModel()
        {
            AboutTrainingProviders = new List<TrainingProviderViewModel>();
        }

        public string InstitutionCode { get; set; }

        public string InstitutionName { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count in training with you")]
        [Required(ErrorMessage = "Enter details about training with you")]
        public string TrainWithUs { get; set; }

        public List<TrainingProviderViewModel> AboutTrainingProviders { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count in training with a disability")]
        [Required(ErrorMessage = "Enter about training with a disability")]
        public string TrainWithDisability { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Enter email address")]
        public string EmailAddress { get; set; }

        [Phone]
        [Required(ErrorMessage = "Enter telephone number")]
        public string Telephone { get; set; }

        [Url]
        [Required(ErrorMessage = "Enter website")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Enter building or street")]
        public string Addr1 { get; set; }

        public string Addr2 { get; set; }

        [Required(ErrorMessage = "Enter town or city")]
        public string Addr3 { get; set; }

        [Required(ErrorMessage = "Enter your county")]
        public string Addr4 { get; set; }

        [Required(ErrorMessage = "Enter postcode")]
        public string Postcode { get; set; }

        public DateTime? LastPublishedTimestampUtc { get; set; }

        public EnumStatus Status { get; set; }

        public static OrganisationViewModel FromEnrichmentModel(UcasInstitutionEnrichmentGetModel ucasInstitutionEnrichmentGetModel, List<TrainingProviderViewModel> aboutAccreditingTrainingProviders, UcasInstitution ucasInstitution)
        {
            ucasInstitutionEnrichmentGetModel = ucasInstitutionEnrichmentGetModel ?? new UcasInstitutionEnrichmentGetModel { EnrichmentModel = new InstitutionEnrichmentModel() { AccreditingProviderEnrichments = new ObservableCollection<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasInstitutionEnrichmentGetModel.EnrichmentModel;
            ucasInstitution = ucasInstitution ?? new UcasInstitution();

            var useUcasContact = 
                string.IsNullOrWhiteSpace(enrichmentModel.Email) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Telephone) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Website) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address1) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address2) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address4) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Address3) &&
                string.IsNullOrWhiteSpace(enrichmentModel.Postcode);

            var result = new OrganisationViewModel
            {
                InstitutionCode = ucasInstitution.InstCode,
                InstitutionName = ucasInstitution.InstFull,
                TrainWithUs = enrichmentModel.TrainWithUs,
                TrainWithDisability = enrichmentModel.TrainWithDisability,
                LastPublishedTimestampUtc = ucasInstitutionEnrichmentGetModel.LastPublishedTimestampUtc,
                Status = ucasInstitutionEnrichmentGetModel.Status,
                AboutTrainingProviders = aboutAccreditingTrainingProviders,

                Addr1 = useUcasContact ? ucasInstitution.Addr1 : enrichmentModel.Address1,
                Addr2 = useUcasContact ? ucasInstitution.Addr2 : enrichmentModel.Address2,
                Addr3 = useUcasContact ? ucasInstitution.Addr3 : enrichmentModel.Address3,
                Addr4 = useUcasContact ? ucasInstitution.Addr4 : enrichmentModel.Address4,
                Postcode = useUcasContact ? ucasInstitution.Postcode : enrichmentModel.Postcode,
                Url = useUcasContact ? ucasInstitution.Url : enrichmentModel.Website,
                Telephone = useUcasContact ? ucasInstitution.Telephone : enrichmentModel.Telephone,
                EmailAddress = useUcasContact ? ucasInstitution.Email : enrichmentModel.Email
            };

            return result;
        }

        
        internal bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(EmailAddress) &&
                string.IsNullOrWhiteSpace(Telephone) &&
                string.IsNullOrWhiteSpace(Url) &&
                string.IsNullOrWhiteSpace(Addr1) &&
                string.IsNullOrWhiteSpace(Addr2) &&
                string.IsNullOrWhiteSpace(Addr4) &&
                string.IsNullOrWhiteSpace(Addr3) &&
                string.IsNullOrWhiteSpace(Postcode) &&
                string.IsNullOrWhiteSpace(TrainWithUs) &&
                string.IsNullOrWhiteSpace(TrainWithDisability) &&
                !AboutTrainingProviders.Any(x => !string.IsNullOrWhiteSpace(x.Description));
        }

    }
}
