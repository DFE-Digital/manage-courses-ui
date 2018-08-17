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

        // TODO: Get this working
        //[Required(ErrorMessage = "Give details about training with you")]
        public string TrainWithUs { get; set; }

        public List<TrainingProviderViewModel> AboutTrainingProviders { get; set; }

        // TODO: Get this working
        //[Required(ErrorMessage = "Give details about training with a disability")]
        public string TrainWithDisability { get; set; }

        public DateTime? LastPublishedTimestampUtc { get; set; }

        public EnumStatus Status { get; set; }
    }
}
