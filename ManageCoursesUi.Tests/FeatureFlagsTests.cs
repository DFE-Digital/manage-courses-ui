using FluentAssertions;
using GovUk.Education.ManageCourses.Ui;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace ManageCoursesUi.Tests{
    [TestFixture]
    public class FeatureFlagsTests{
        [Test]
        public void TestFlagsWithNoConfiguration(){
            var config = new Mock<IConfigurationSection>();
            IFeatureFlags flags = new FeatureFlags(config.Object);
            flags.TestFlagEnabled.Should().BeFalse("No configuration present, should default to off without error.");
        }
    }
}