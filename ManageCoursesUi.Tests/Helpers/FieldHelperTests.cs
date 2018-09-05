
using NUnit.Framework;
using GovUk.Education.ManageCourses.Ui.Helpers;
using FluentAssertions;

namespace ManageCoursesUi.Tests.Helpers
{
    [TestFixture]
    public class FieldHelperTests
    {
        [Test]
        public void Integer()
        {
            int? normal = 123;
            int? negative = int.MinValue;
            int? veryLarge = int.MaxValue;

            normal.DisplayText().Should().Be("£123");
            negative.DisplayText().Should().Be("£-2,147,483,648");
            veryLarge.DisplayText().Should().Be("£2,147,483,647");
            veryLarge.DisplayText(6).Should().Be("£2,147");
        }
    }
}
