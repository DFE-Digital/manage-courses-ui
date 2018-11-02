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

        public string InstCode { get; set; }

        public string InstName { get; set; }

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
                InstCode = model.InstCode,
                InstName = model.InstName,
                TrainWithUs = model.TrainWithUs,
                AboutTrainingProviders = model.AboutTrainingProviders,
                TrainWithDisability = model.TrainWithDisability,
                LastPublishedTimestampUtc = model.LastPublishedTimestampUtc,
                Status = model.Status,
            };
        }

        public void MergeIntoEnrichmentModel(ref InstitutionEnrichmentModel enrichmentModel)
        {
            if (enrichmentModel == null)
            {
                enrichmentModel = new InstitutionEnrichmentModel();
            }

            var aboutTrainingProviders = new List<AccreditingProviderEnrichment>(
                AboutTrainingProviders.Select(x => new AccreditingProviderEnrichment
                {
                    UcasInstitutionCode = x.InstCode,
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
