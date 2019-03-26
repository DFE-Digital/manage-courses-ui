using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    /// <summary>
    /// This model is used for showing the summary page of the organisation Enrichment,
    /// and for publishing. The validation covers required fields. For individual enrichment
    /// editor pages, see <see cref="OrganisationViewModelForAbout" /> and <see cref="OrganisationViewModelForContact" />
    /// </summary>
    public class OrganisationViewModel
    {
        public OrganisationViewModel()
        {
            AboutTrainingProviders = new List<TrainingProviderViewModel>();
        }

        public string ProviderCode { get; set; }

        public string ProviderName { get; set; }

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

        [MinLength(7, ErrorMessage = "Postcode is too short. Enter a postcode in the format ‘SW10 1AA’")]
        [MaxLength(8, ErrorMessage = "Postcode is too long. Enter a postcode in the format ‘SW10 1AA’")]
        [Required(ErrorMessage = "Enter a postcode in the format ‘SW10 1AA’")]
        public string Postcode { get; set; }

        public DateTime? LastPublishedTimestampUtc { get; set; }

        public EnumStatus Status { get; set; }

        public static OrganisationViewModel FromEnrichmentModel(UcasProviderEnrichmentGetModel ucasProviderEnrichmentGetModel, List<TrainingProviderViewModel> aboutAccreditingTrainingProviders, ProviderSummary providerSummary)
        {
            ucasProviderEnrichmentGetModel = ucasProviderEnrichmentGetModel ?? new UcasProviderEnrichmentGetModel { EnrichmentModel = new ProviderEnrichmentModel() { AccreditingProviderEnrichments = new List<AccreditingProviderEnrichment>() } };

            var enrichmentModel = ucasProviderEnrichmentGetModel.EnrichmentModel;

            var result = new OrganisationViewModel
            {
                ProviderCode = providerSummary.ProviderCode,
                ProviderName = providerSummary.ProviderName,
                TrainWithUs = enrichmentModel.TrainWithUs,
                TrainWithDisability = enrichmentModel.TrainWithDisability,
                LastPublishedTimestampUtc = ucasProviderEnrichmentGetModel.LastPublishedTimestampUtc,
                Status = ucasProviderEnrichmentGetModel.Status,
                AboutTrainingProviders = aboutAccreditingTrainingProviders,

                Addr1 = enrichmentModel.Address1,
                Addr2 = enrichmentModel.Address2,
                Addr3 = enrichmentModel.Address3,
                Addr4 = enrichmentModel.Address4,
                Postcode = enrichmentModel.Postcode,
                Url = enrichmentModel.Website,
                Telephone = enrichmentModel.Telephone,
                EmailAddress = enrichmentModel.Email
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
