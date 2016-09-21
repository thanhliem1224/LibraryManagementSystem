using LibraryManagementSystem.DAL;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LibraryManagementSystem.Controllers
{

    public class HomeController : Controller
    {


        private CNCFContext db = new CNCFContext();
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
    }
}