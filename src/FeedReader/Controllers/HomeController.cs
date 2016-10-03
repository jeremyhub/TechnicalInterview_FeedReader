﻿using System.Web.Mvc;

namespace FeedReader.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("ListAllItems", "Rss");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}