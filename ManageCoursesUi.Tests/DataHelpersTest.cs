using System;
using System.Collections.Generic;
using System.Text;
using GovUk.Education.ManageCourses.Ui.Helpers;
using GovUk.Education.ManageCourses.Ui.ViewModels;
using NUnit.Framework;

namespace ManageCoursesUi.Tests
{    
    [TestFixture]
    class DataHelpersTest
    {
        private const string AgeRangeSResult = "Secondary (11+ years)";
        private const string AgeRangeMResult = "Middle years (7 - 14 years)";
        private const string AgeRangePResult = "Primary (3 - 11/12 years)";
        private const string QualificationPfResult = "Professional";
        private const string QualificationPgResult = "Postgraduate";
        private const string QualificationBoResult = "Professional/Postgraduate";
        private const string QualificationEmptyResult = "Recommendation for QTS";
        private const string IncorrectResult = "";

        [Test]
        [TestCase("s")]
        [TestCase("S")]
        [TestCase("s ")]
        [TestCase("  s ")]
        [TestCase("S ")]
        [TestCase("  S ")]
        public void GetAgeRange_should_be_correct_for_s(string input)
        {
            var param = new CourseVariantViewModel{AgeRange = input};

            var result = param.GetAgeRange();

            Assert.IsTrue(result == AgeRangeSResult);
        }
        [Test]
        [TestCase("m")]
        [TestCase("M")]
        [TestCase("m ")]
        [TestCase("  m ")]
        [TestCase("M ")]
        [TestCase("  M ")]
        public void GetAgeRange_should_be_correct_for_m(string input)
        {
            var param = new CourseVariantViewModel { AgeRange = input };

            var result = param.GetAgeRange();

            Assert.IsTrue(result == AgeRangeMResult);
        }
        [Test]
        [TestCase("p")]
        [TestCase("P")]
        [TestCase("p ")]
        [TestCase("  p ")]
        [TestCase("P ")]
        [TestCase("  P ")]
        public void GetAgeRange_should_be_correct_for_p(string input)
        {
            var param = new CourseVariantViewModel { AgeRange = input };

            var result = param.GetAgeRange();

            Assert.IsTrue(result == AgeRangePResult);
        }
        [Test]
        [TestCase("")]
        [TestCase("weroiweroi weroiu weroiu weroiu")]
        [TestCase("   ")]
        [TestCase(null)]
        public void GetAgeRange_should_be_empty(string input)
        {
            var param = new CourseVariantViewModel { AgeRange = input };

            var result = param.GetAgeRange();

            Assert.IsTrue(result == IncorrectResult);
        }

        [Test]
        [TestCase("pf")]
        [TestCase("PF")]
        [TestCase("pF ")]
        [TestCase("  pf ")]
        [TestCase("PF ")]
        [TestCase("Pf")]
        [TestCase("pf ")]
        [TestCase("  pF ")]
        [TestCase("Pf ")]
        [TestCase("pF")]
        [TestCase("Pf ")]
        public void GetQualification_should_be_correct_for_pf(string input)
        {
            var param = new CourseVariantViewModel { Qualifications = input };

            var result = param.GetQualification();

            Assert.IsTrue(result == QualificationPfResult);
        }
        [Test]
        [TestCase("pg")]
        [TestCase("PG")]
        [TestCase("pG ")]
        [TestCase("  pg ")]
        [TestCase("PG ")]
        [TestCase("Pg")]
        [TestCase("pg ")]
        [TestCase("  pG ")]
        [TestCase("Pg ")]
        [TestCase("pG")]
        [TestCase("Pg ")]
        public void GetQualification_should_be_correct_for_pg(string input)
        {
            var param = new CourseVariantViewModel { Qualifications = input };

            var result = param.GetQualification();

            Assert.IsTrue(result == QualificationPgResult);
        }
        [Test]
        [TestCase("bo")]
        [TestCase("BO")]
        [TestCase("bO ")]
        [TestCase("  bo ")]
        [TestCase("BO ")]
        [TestCase("Bo")]
        [TestCase("  bO ")]
        [TestCase("bO")]
        [TestCase("bo ")]
        [TestCase("  bO ")]
        [TestCase("Bo ")]
        public void GetQualification_should_be_correct_for_bo(string input)
        {
            var param = new CourseVariantViewModel { Qualifications = input };

            var result = param.GetQualification();

            Assert.IsTrue(result == QualificationBoResult);
        }
         [Test]
        [TestCase("")]
        [TestCase("   ")]
        public void GetQualification_should_be_correct_for_empty(string input)
        {
            var param = new CourseVariantViewModel { Qualifications = input };

            var result = param.GetQualification();

            Assert.IsTrue(result == QualificationEmptyResult);
        }
        [Test]
        [TestCase(null)]
        [TestCase("qweoiuqweoiuqw qwoeiu qweoiu qweiu")]
        public void GetQualification_should_be_empty(string input)
        {
            var param = new CourseVariantViewModel { Qualifications = input };

            var result = param.GetQualification();

            Assert.IsTrue(result == IncorrectResult);
        }
    }
}
