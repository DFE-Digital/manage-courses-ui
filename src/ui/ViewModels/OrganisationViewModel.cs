using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class OrganisationViewModel : TabbedViewModel
    {
        public OrganisationViewModel()
        {
            AboutTrainingProviders = new List<TrainingProviderViewModel>();
        }

        public string InstitutionCode { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for details about training with you")]
        [Required(ErrorMessage = "Give details about training with you")]
        public string TrainWithUs { get; set; }

        public List<TrainingProviderViewModel> AboutTrainingProviders { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for details about training with a disability")]
        [Required(ErrorMessage = "Give details about training with a disability")]
        public string TrainWithDisability { get; set; }

        public DateTime? LastPublishedTimestampUtc { get; set; }

        public EnumStatus Status { get; set; }

        public bool PublishOrganisation { get; set; }
    }
}
