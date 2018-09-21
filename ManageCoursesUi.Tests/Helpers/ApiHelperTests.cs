using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using FluentAssertions;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui.Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace ManageCoursesUi.Tests.Helpers
{
    [TestFixture()]
    public class ApiHelperTests
    {
        [Test]
        public void TestConvert()
        {
            //setup
            var courseToConvert =
                new GovUk.Education.ManageCourses.ApiClient.Course2
                {
                    Campuses = new ObservableCollection<Campus2> {new Campus2()},
                    AccreditingProvider = new Provider {Id = 123},
                    AgeRange = AgeRange.Primary,
                    AccreditingProviderId = 123,
                    ApplicationsAcceptedFrom = DateTime.Today,
                    ContactDetails = new Contact {Id = 1, Address = "address", Email = "email"},
                    ContactDetailsId = 1,
                    CourseSubjects = new ObservableCollection<CourseSubject> {new CourseSubject {CourseId = 1}},
                    DescriptionSections =
                        new ObservableCollection<CourseDescriptionSection>
                        {
                            new CourseDescriptionSection {CourseId = 1, Name = "name"}
                        },
                    Distance = 12.5,
                    Duration = "Six months",
                    Fees = new Fees {Eu = 123, International = 234, Uk = 345}
                };

            var convertedCourse = ApiHelper.Convert(courseToConvert);
            //assert
            convertedCourse.Campuses.Count.Should().Be(courseToConvert.Campuses.Count);
            convertedCourse.AccreditingProvider.Id.Should().Be(courseToConvert.AccreditingProviderId);
            convertedCourse.AgeRange.Should().BeEquivalentTo(courseToConvert.AgeRange);
            convertedCourse.AccreditingProviderId.Should().Be(courseToConvert.AccreditingProviderId);
            convertedCourse.ApplicationsAcceptedFrom.Should().Be(courseToConvert.ApplicationsAcceptedFrom);
            convertedCourse.ContactDetails.Address.Should().Be(courseToConvert.ContactDetails.Address);
            convertedCourse.ContactDetailsId.Should().Be(courseToConvert.ContactDetailsId);
            convertedCourse.CourseSubjects.Count.Should().Be(courseToConvert.CourseSubjects.Count);
            convertedCourse.DescriptionSections.Count.Should().Be(courseToConvert.DescriptionSections.Count);
            convertedCourse.Distance.Should().Be(courseToConvert.Distance);
            convertedCourse.Duration.Should().Be(courseToConvert.Duration);
            courseToConvert.Fees.Eu.Should().Be(courseToConvert.Fees.Eu);
            courseToConvert.Fees.International.Should().Be(courseToConvert.Fees.International);
            courseToConvert.Fees.Uk.Should().Be(courseToConvert.Fees.Uk);
        }
    }
}
