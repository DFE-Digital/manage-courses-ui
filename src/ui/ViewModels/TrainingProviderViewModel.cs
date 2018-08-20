using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
  public class TrainingProviderViewModel
  {
    public string InstitutionName { get; set; }
    public string InstitutionCode { get; set; }

    [RegularExpression(@"^\s*(\S+\s+|\S+$){0,100}$", ErrorMessage = "Too many words")]
    public string Description { get; set; }
  }
}
