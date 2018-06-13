﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageCoursesUi.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Courses");
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}