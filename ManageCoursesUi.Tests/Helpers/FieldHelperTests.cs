
using NUnit.Framework;
using GovUk.Education.ManageCourses.Ui.Helpers;
using FluentAssertions;

namespace ManageCoursesUi.Tests.Helpers
{
    [TestFixture]
    public class FieldHelperTests
    {        
        [Test]
        public void Decimal()
        {
            decimal? normal = 123.45M;
            decimal? negative = -123.45M;
            decimal? veryLarge = 10000000000000000000000000000.00M;

            normal.DisplayText().Should().Be("£123");
            negative.DisplayText().Should().Be("£-123");
            veryLarge.DisplayText().Should().Be("£10,000,000,000,000,000,000,000,000,000");
            veryLarge.DisplayText(3).Should().Be("£10");
        }
    }
}
