using System.ComponentModel.DataAnnotations;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
  public class TrainingProviderViewModel
  {
    public string InstitutionName { get; set; }
    public string InstitutionCode { get; set; }

    public string Description { get; set; }

    //Custom validation regex and message as the message needs to be customised and bypass the normal validation route..
    public string ValidationMessage { get { return $"Reduce word count for {InstitutionName}"; } }
    public string ValidationRegex { get { return @"^\s*(\S+\s+|\S+$){0,100}$"; } }
  }
}
