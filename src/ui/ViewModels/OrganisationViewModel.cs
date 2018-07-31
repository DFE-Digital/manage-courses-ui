using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
  public class OrganisationViewModel : TabbedViewModel
  {
    [Display(Name = "Train with us")]
    public string TrainWithUs { get; set; }

    [Display(Name = "Website")]
    public string DomainName { get; set; }

    [Display(Name = "About the training provider")]
    public string AboutTrainingProvider { get; set; }

    [Display(Name = "Train with disability")]
    public string TrainWithDisability { get; set; }
  }
}
