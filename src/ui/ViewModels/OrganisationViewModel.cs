using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
  public class OrganisationViewModel : TabbedViewModel
  {
    // public OrganisationViewModel()
    // {
    //   AboutTrainingProviders = new List<AboutAccreditingProviderViewModel>();
    // }

    [Display(Name = "Training with you")]
    public string TrainWithUs { get; set; }

    [Display(Name = "Website")]
    public string DomainName { get; set; }

    [Display(Name = "About the provider (optional)")]
    public string AboutTrainingProvider { get; set; }

    [Display(Name = "Training with a disability")]
    public string TrainWithDisability { get; set; }

    //public List<AboutAccreditingProviderViewModel> AboutTrainingProviders { get; set; }
  }

  // public class AboutAccreditingProviderViewModel
  // {
  //   public string ProviderCode { get; set; }
  //   public string DisplayName { get; set; }
  //   public string Description { get; set; }
  // }
}
