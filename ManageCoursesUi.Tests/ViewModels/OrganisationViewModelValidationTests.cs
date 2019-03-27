using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;
using NUnit.Framework;


namespace ManageCoursesUi.Tests.ViewModels
{
    [TestFixture]
    public class OrganisationViewModelValidationTests
    {

        [TestCase("SW10 1AA")]
        [TestCase("SW101AA")]
        [TestCase("Sw101Aa")]
        [TestCase("sw101aa")]
        [TestCase("sw10 1aa")]
        [TestCase("s1 1aa")]
        [TestCase("S1 1AA")]
        [TestCase("S11AA")]
        public void PostCode_Valid(string postcode)
        {
            var ovm = GetMissingPostCodeModel();
            ovm.Postcode = postcode;
            var validationResults = ValidateModel(ovm);
            validationResults.Should().HaveCount(0);
        }

        [Test]
        public void PostCode_RegularExpression()
        {
            var ovm = GetMissingPostCodeModel();
            ovm.Postcode = "*****";
            var validationResults = ValidateModel(ovm);
            validationResults.Should().HaveCount(1);
            validationResults[0].ErrorMessage.Should().Be("Enter a postcode in the format ‘SW10 1AA‘");
        }

        [Test]
        public void PostCode_MaxLength()
        {
            var ovm = GetMissingPostCodeModel();
            ovm.Postcode = "123456789";
            var validationResults = ValidateModel(ovm);
            validationResults.Should().HaveCount(1);
            validationResults[0].ErrorMessage.Should().Be("Postcode is too long. Enter a postcode in the format ‘SW10 1AA’");
        }

        [Test]
        public void PostCode_MinLength()
        {
            var ovm = GetMissingPostCodeModel();
            ovm.Postcode = "1";
            var validationResults = ValidateModel(ovm);
            validationResults.Should().HaveCount(1);
            validationResults[0].ErrorMessage.Should().Be("Postcode is too short. Enter a postcode in the format ‘SW10 1AA’");
        }


        [Test]
        public void PostCode_Required()
        {
            var ovm = GetMissingPostCodeModel();

            var validationResults = ValidateModel(ovm);
            validationResults.Should().HaveCount(1);
            validationResults[0].ErrorMessage.Should().Be("Enter a postcode in the format ‘SW10 1AA’");
        }

        private OrganisationViewModel GetMissingPostCodeModel()
        {
            return new OrganisationViewModel
                {

                    TrainWithUs = "TrainWithUs",
                    TrainWithDisability = "TrainWithDisability",
                    EmailAddress = "email@example.org",
                    Telephone = "1234",
                    Url = "http://example.org",
                    Addr1 = "Addr1",
                    Addr3 = "Addr3",
                    Addr4 = "Addr4",
                };
        }
        private IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }
    }
}
