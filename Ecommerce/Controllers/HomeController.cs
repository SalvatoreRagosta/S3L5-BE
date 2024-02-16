using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Epicode";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Salvatore Ragosta";

            return View();
        }
    }
}