using System;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.ApiClient;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class OrganisationViewModelForContact
    {        
        public string InstitutionCode { get; set; }

        public string InstitutionName { get; set; }


        [EmailAddress]
        public string EmailAddress { get; set; }

        [Phone]
        public string Telephone { get; set; }

        [Url]
        public string Url { get; set; }

        public string Addr1 { get; set; }

        public string Addr2 { get; set; }

        public string Addr3 { get; set; }

        public string Addr4 { get; set; }

        public string Postcode { get; set; }
        
        public DateTime? LastPublishedTimestampUtc { get; set; }

        public EnumStatus Status { get; set; }

        public static OrganisationViewModelForContact FromGeneralViewModel(OrganisationViewModel model)
        {
            return new OrganisationViewModelForContact
            {
                InstitutionCode = model.InstitutionCode,
                InstitutionName = model.InstitutionName,
                EmailAddress = model.EmailAddress,
                Telephone = model.Telephone,
                Url = model.Url,
                Addr1 = model.Addr1,
                Addr2 = model.Addr2,
                Addr3 = model.Addr3,
                Addr4 = model.Addr4,
                Postcode = model.Postcode,
                LastPublishedTimestampUtc = model.LastPublishedTimestampUtc,
                Status = model.Status                
            };
        }

        public bool IsEmpty()
        {
            return 
                string.IsNullOrWhiteSpace(EmailAddress) &&
                string.IsNullOrWhiteSpace(Telephone) &&
                string.IsNullOrWhiteSpace(Url) &&
                string.IsNullOrWhiteSpace(Addr1) &&
                string.IsNullOrWhiteSpace(Addr2) &&
                string.IsNullOrWhiteSpace(Addr3) &&
                string.IsNullOrWhiteSpace(Addr4) &&
                string.IsNullOrWhiteSpace(Postcode);                
        }

        public void MergeIntoEnrichmentModel(ref InstitutionEnrichmentModel enrichmentModel)
        {
            if (enrichmentModel == null)
            {
                enrichmentModel = new InstitutionEnrichmentModel();
            }

            enrichmentModel.Email = EmailAddress;
            enrichmentModel.Telephone = Telephone;
            enrichmentModel.Website = Url;
            enrichmentModel.Address1 = Addr1;
            enrichmentModel.Address2 = Addr2;
            enrichmentModel.Address3 = Addr3;
            enrichmentModel.Address4 = Addr4;
            enrichmentModel.Postcode = Postcode;            
        }
        
        public static bool IsContactProperty(string property)
        {
            return Array.IndexOf(new string[] {
                nameof(EmailAddress),
                nameof(Telephone),
                nameof(Url),
                nameof(Addr1),
                nameof(Addr2),
                nameof(Addr3),
                nameof(Addr4),
                nameof(Postcode)
            }, property) > -1;
        }
    }
}
