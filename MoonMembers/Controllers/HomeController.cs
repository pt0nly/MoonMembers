﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoonMembers.Controllers
{
    public class HomeController : Controller
    {
        #region FrontOffice

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #endregion

        #region BackOffice

        [Route("backoffice", Name = "BackofficeHome")]
        public ActionResult BackofficeIndex()
        {
            return View("Backoffice/Index");
        }

        #endregion
    }
}