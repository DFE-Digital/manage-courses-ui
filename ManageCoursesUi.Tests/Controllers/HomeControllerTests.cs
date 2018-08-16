
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GovUk.Education.ManageCourses.ApiClient;
using GovUk.Education.ManageCourses.Ui;
using GovUk.Education.ManageCourses.Ui.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace ManageCoursesUi.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController sut;
        private Mock<IManageApi> mockApi;

        [SetUp]
        public void SetUp() 
        {
            mockApi = new Mock<IManageApi>();
            sut = new HomeController(mockApi.Object); 
        } 

        [Test]
        public void Index_IfNoOrgs_Returns401()
        {
            mockApi.Setup(x => x.GetOrganisations())
                .Returns(Task.FromResult((IEnumerable<UserOrganisation>) new List<UserOrganisation>()));
            
            var res = sut.Index().Result;

            Assert.IsTrue(res is StatusCodeResult);
            Assert.AreEqual(401, (res as StatusCodeResult).StatusCode);
        }
        
        [Test]
        public void Index_IfApiThrows401_Returns401()
        {
            mockApi.Setup(x => x.GetOrganisations())
                .ThrowsAsync(new SwaggerException("uh-oh...", 401, "", new Dictionary<string, IEnumerable<string>>(), new Exception("inner")));
            
            var res = sut.Index().Result;

            Assert.IsTrue(res is StatusCodeResult);
            Assert.AreEqual(401, (res as StatusCodeResult).StatusCode);
        }
    }
}