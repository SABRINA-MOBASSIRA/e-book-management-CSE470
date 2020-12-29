using Microsoft.AspNet.Identity;
using EBM.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace EBM.Controllers
{
    public class HomeController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        public ActionResult Index()
        {
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            PermittedGroupRoll pgr = new PermittedGroupRoll();

            if (string.IsNullOrEmpty(userID))
            {
                AuthenticationManager.SignOut();
                return RedirectToAction("Login", "Account");
            }
            System.Web.HttpContext.Current.Session["PermittedGroupRoll"] = pgr.loadUserGroupRoll(userID);
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
    }
}