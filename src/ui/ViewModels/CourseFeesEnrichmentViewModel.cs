using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels.Enums;

namespace GovUk.Education.ManageCourses.Ui.ViewModels
{
    public class CourseFeesEnrichmentViewModel : ICourseEnrichmentViewModel
    {
        public CourseLength? CourseLength { get; set; }
        public string CourseLengthInput { get; set; }

        [Range(FieldHelper.FeeMin, FieldHelper.FeeMax, ErrorMessage = "UK course fee must be less than £100,000")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "UK course fee must contain numbers only")]
        public int? FeeUkEu { get; set; }

        [Range(FieldHelper.FeeMin, FieldHelper.FeeMax, ErrorMessage = "International course fee must be less than £100,000")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "International course fee must contain numbers only")]
        public int? FeeInternational { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for fee details")]
        public string FeeDetails { get; set; }

        [RegularExpression(@"^\s*(\S+\s+|\S+$){0,250}$", ErrorMessage = "Reduce the word count for financial support")]
        public string FinancialSupport { get; set; }

        public CourseRouteDataViewModel RouteData { get; set; }

        public CourseInfoViewModel CourseInfo { get; set; }

        public bool IsEmpty() =>
            !CourseLength.HasValue &&
            !FeeUkEu.HasValue &&
            !FeeInternational.HasValue &&
            string.IsNullOrEmpty(FeeDetails) &&
            string.IsNullOrEmpty(FinancialSupport);

        public void MapInto(ref CourseEnrichmentModel enrichmentModel)
        {
            var courseLength = CourseLength.HasValue ? CourseLength.Value.ToString() : null;
            enrichmentModel.CourseLength = courseLength == "Other" ? (string.IsNullOrEmpty(CourseLengthInput) ? null : CourseLengthInput) : courseLength;

            enrichmentModel.FeeUkEu = FeeUkEu;
            enrichmentModel.FeeInternational = FeeInternational;
            enrichmentModel.FeeDetails = FeeDetails;
            enrichmentModel.FinancialSupport = FinancialSupport;
        }

        public IEnumerable<CopiedField> CopyFrom(CourseEnrichmentModel model)
        {
            var res = new List<CopiedField>();

            if (model == null)
            {
                return res;
            }

            if (Enum.TryParse(model.CourseLength ?? "", out CourseLength courseLength))
            {
                CourseLength = courseLength;
                res.Add(new CopiedField(nameof(model.CourseLength), CourseEnrichmentFieldNames.CourseLength));
            }
            if (!CourseLength.HasValue && !string.IsNullOrEmpty(model.CourseLength))
            {
                CourseLengthInput = model.CourseLength;
                CourseLength = Enums.CourseLength.Other;
            }
            if (model.FeeUkEu.HasValue)
            {
                FeeUkEu = model.FeeUkEu.GetFeeValue();
                res.Add(new CopiedField(nameof(model.FeeUkEu), CourseEnrichmentFieldNames.FeesUkEu));
            }

            if (model.FeeInternational.HasValue)
            {
                FeeInternational = model.FeeInternational.GetFeeValue();
                res.Add(new CopiedField(nameof(model.FeeInternational), CourseEnrichmentFieldNames.FeesInternational));
            }

            if (!string.IsNullOrEmpty(model.FeeDetails))
            {
                FeeDetails = model.FeeDetails;
                res.Add(new CopiedField(nameof(model.FeeDetails), CourseEnrichmentFieldNames.FeeDetails));
            }

            if (!string.IsNullOrEmpty(model.FinancialSupport))
            {
                FinancialSupport = model.FinancialSupport;
                res.Add(new CopiedField(nameof(model.FinancialSupport), CourseEnrichmentFieldNames.FinancialSupport));
            }

            return res;
        }
    }
}
