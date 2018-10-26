using System;
using System.Collections.Generic;
using System.Net;
using GovUk.Education.ManageCourses.Api.Model;
using GovUk.Education.ManageCourses.Domain.Models;
using GovUk.Education.ManageCourses.Ui.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace ManageCoursesUi.Tests
{
    [TestFixture]
    public class McExceptionFilterTests
    {
        private McExceptionFilter sut;

        [SetUp]
        public void SetUp()
        {
            sut = new McExceptionFilter(new Mock<ILogger<McExceptionFilter>>().Object);
        }

        [Test]
        public void OnException()
        {
            var exceptionContext = GetExceptionContext();

            exceptionContext.Exception = new Exception();
            Assert.IsNotNull(exceptionContext.Exception);
            Assert.IsNull(exceptionContext.Result);

            sut.OnException(exceptionContext);

            Assert.IsTrue(exceptionContext.ExceptionHandled);
            Assert.IsNotNull(exceptionContext.Exception);
            Assert.IsNotNull(exceptionContext.Result as ViewResult);
            Assert.AreEqual(500, (exceptionContext.Result as ViewResult).StatusCode);
        }

        [Test]
        public void OnException_SwaggerException()
        {
            var exceptionContext = GetExceptionContext();

            exceptionContext.Exception = new SwaggerException("SwaggerException", 123, "SwaggerException", null, null);
            Assert.IsNotNull(exceptionContext.Exception);
            Assert.IsNull(exceptionContext.Result);

            sut.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Exception);
            Assert.IsNotNull(exceptionContext.Result as ViewResult);
            Assert.AreEqual(500, (exceptionContext.Result as ViewResult).StatusCode);
            Assert.True(exceptionContext.ExceptionHandled);

        }

        [Test]
        public void OnException_SwaggerException_Unauthorized()
        {
            var exceptionContext = GetExceptionContext();

            exceptionContext.Exception = new SwaggerException("SwaggerException", (int)HttpStatusCode.Unauthorized, "SwaggerException", null, null);
            Assert.IsNotNull(exceptionContext.Exception);
            Assert.IsNull(exceptionContext.Result);

            sut.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Exception);
            Assert.IsNotNull(exceptionContext.Result);
            Assert.IsTrue(exceptionContext.ExceptionHandled);

            var viewResult = exceptionContext.Result as ViewResult;
            Assert.IsNotNull(viewResult);
            Assert.AreEqual("~/Views/Error/Index.cshtml", viewResult.ViewName);
            Assert.AreEqual(401, viewResult.StatusCode);
        }

        [Test]
        public void OnException_SwaggerException_UnavailableForLegalReasons()
        {
            var exceptionContext = GetExceptionContext();

            exceptionContext.Exception = new SwaggerException("SwaggerException", 451, "SwaggerException", null, null);
            Assert.IsNotNull(exceptionContext.Exception);
            Assert.IsNull(exceptionContext.Result);

            sut.OnException(exceptionContext);

            Assert.IsNotNull(exceptionContext.Exception);
            Assert.IsNotNull(exceptionContext.Result);
            Assert.IsTrue(exceptionContext.ExceptionHandled);

            var redirectToActionResult = exceptionContext.Result as RedirectToActionResult;
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual("AcceptTerms", redirectToActionResult.ActionName);
            Assert.AreEqual("Home", redirectToActionResult.ControllerName);
            Assert.IsNull(redirectToActionResult.RouteValues);
        }

        private static ExceptionContext GetExceptionContext()
        {
            var httpContext = new Mock<HttpContext>();

            var actionContext = new ActionContext(
                httpContext.Object,
                new Mock<RouteData>().Object,
                new Mock<ActionDescriptor>().Object
            );

            var result = new ExceptionContext(
                actionContext,
                new List<IFilterMetadata>()
            );

            return result;
        }
    }
}
