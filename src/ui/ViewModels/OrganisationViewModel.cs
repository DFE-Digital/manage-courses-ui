using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
  public class OrganisationViewModel : TabbedViewModel
  {
    [Display(Name = "Training with you")]
    public string TrainWithUs { get; set; }

    [Display(Name = "Website")]
    public string DomainName { get; set; }

    [Display(Name = "About the provider (optional)")]
    public string AboutTrainingProvider { get; set; }

    [Display(Name = "Training with a disability")]
    public string TrainWithDisability { get; set; }
  }
}
