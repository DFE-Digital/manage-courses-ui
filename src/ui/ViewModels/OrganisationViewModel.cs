using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
  public class OrganisationViewModel : TabbedViewModel
  {
    public int Id { get; set; }

    public string InstitutionCode { get; set; }

    [Display(Name = "Training with you")]
    public string TrainWithUs { get; set; }

    [Display(Name = "Website")]
    public string DomainName { get; set; }

    [Display(Name = "About the provider (optional)")]
    public List<TrainingProviderViewModel> AboutTrainingProviders { get; set; }

    [Display(Name = "Training with a disability")]
    public string TrainWithDisability { get; set; }
  }
}
