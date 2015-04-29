using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CollaboratR.Controllers
{
    //Handle error annotation lets us write custom error handlers for this controller.
    [HandleError()]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Collaboratr Mission Statement";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "We want your feedback!";

            return View();
        }

        public ActionResult UserGuide()
        {
            ViewBag.Message = "New to collaboratr?";

            return View();
        }
        public ActionResult PrivacyPolicy()
        {
            ViewBag.Message = "Privacy Policy";

            return View();
        }
        public ActionResult Terms()
        {
            ViewBag.Message = "Terms of Service";

            return View();
        }
        public ActionResult UserManual()
        {
            ViewBag.Message = "User Manual & FAQ";

            return View();
        }

    }
}