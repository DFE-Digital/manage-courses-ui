using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class OrganisationViewModel
    {
        public OrganisationViewModel()
        {
            AboutTrainingProviders = new List<TrainingProviderViewModel>();
        }

        public string InstitutionCode { get; set; }

        public string InstitutionName { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count in training with you")]
        [Required(ErrorMessage = "Enter details about training with you")]
        public string TrainWithUs { get; set; }

        public List<TrainingProviderViewModel> AboutTrainingProviders { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count in training with a disability")]
        [Required(ErrorMessage = "Enter about training with a disability")]
        public string TrainWithDisability { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Enter email address")]
        public string EmailAddress { get; set; }

        [Phone]
        [Required(ErrorMessage = "Enter telephone number")]
        public string Telephone { get; set; }

        [Url]
        [Required(ErrorMessage = "Enter website")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Enter building or street")]
        public string Addr1 { get; set; }

        public string Addr2 { get; set; }

        [Required(ErrorMessage = "Enter town or city")]
        public string Addr3 { get; set; }

        [Required(ErrorMessage = "Enter your county")]
        public string Addr4 { get; set; }

        [Required(ErrorMessage = "Enter postcode")]
        public string Postcode { get; set; }

        public DateTime? LastPublishedTimestampUtc { get; set; }

        public EnumStatus Status { get; set; }

        public bool PublishOrganisation { get; set; }
    }
}
