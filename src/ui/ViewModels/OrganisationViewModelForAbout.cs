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
    public class OrganisationViewModelForAbout
    {
        public OrganisationViewModelForAbout()
        {
            AboutTrainingProviders = new List<TrainingProviderViewModel>();
        }

        public string ProviderCode { get; set; }

        public string ProviderName { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count in training with you")]
        public string TrainWithUs { get; set; }

        public List<TrainingProviderViewModel> AboutTrainingProviders { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count in training with a disability")]
        public string TrainWithDisability { get; set; }

        public DateTime? LastPublishedTimestampUtc { get; set; }

        public EnumStatus Status { get; set; }

        public static OrganisationViewModelForAbout FromGeneralViewModel(OrganisationViewModel model)
        {
            return new OrganisationViewModelForAbout
            {
                ProviderCode = model.ProviderCode,
                ProviderName = model.ProviderName,
                TrainWithUs = model.TrainWithUs,
                AboutTrainingProviders = model.AboutTrainingProviders,
                TrainWithDisability = model.TrainWithDisability,
                LastPublishedTimestampUtc = model.LastPublishedTimestampUtc,
                Status = model.Status,
            };
        }

        public void MergeIntoEnrichmentModel(ref ProviderEnrichmentModel enrichmentModel)
        {
            if (enrichmentModel == null)
            {
                enrichmentModel = new ProviderEnrichmentModel();
            }

            var aboutTrainingProviders = new List<AccreditingProviderEnrichment>(
                AboutTrainingProviders.Select(x => new AccreditingProviderEnrichment
                {
                    UcasProviderCode = x.ProviderCode,
                    Description = x.Description
                }));

            enrichmentModel.TrainWithUs = TrainWithUs;
            enrichmentModel.AccreditingProviderEnrichments = aboutTrainingProviders;
            enrichmentModel.TrainWithDisability = TrainWithDisability;
        }

        internal bool IsEmpty()
        {
            return string.IsNullOrEmpty(TrainWithUs)
                && string.IsNullOrEmpty(TrainWithDisability)
                && !AboutTrainingProviders.Any(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}
