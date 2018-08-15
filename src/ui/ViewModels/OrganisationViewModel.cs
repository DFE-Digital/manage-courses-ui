using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class OrganisationViewModel : TabbedViewModel
    {
        public OrganisationViewModel()
        {
            AboutTrainingProviders = new List<TrainingProviderViewModel>();
        }

        public string InstitutionCode { get; set; }

        public string TrainWithUs { get; set; }

        public List<TrainingProviderViewModel> AboutTrainingProviders { get; set; }

        public string TrainWithDisability { get; set; }
    }
}
