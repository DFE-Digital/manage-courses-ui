using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class WordCountOrganisationViewModel
    {
        public WordCountOrganisationViewModel(OrganisationViewModel viewModel)
        {
            this.TrainWithUs = viewModel.TrainWithUs;
            this.TrainWithDisability = viewModel.TrainWithDisability;
            this.AboutTrainingProviders = viewModel.AboutTrainingProviders;
        }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for details about training with you")]
        public virtual string TrainWithUs { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for details about training with a disability")]
        public virtual string TrainWithDisability { get; set; }

        public List<TrainingProviderViewModel> AboutTrainingProviders { get; set; }
    }
}
