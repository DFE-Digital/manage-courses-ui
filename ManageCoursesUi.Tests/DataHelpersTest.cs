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
        [OneTimeSetUp]
        public void Setup()
        {

        }

        [Test]
        public void AgeRange_should_be_correct_for_s()
        {
            var param = new CourseVariantViewModel{AgeRange = "s"};

            var result = param.GetAgeRange();

            Assert.IsTrue(result == "Secondary (11+ years)");
        }
    }
}
