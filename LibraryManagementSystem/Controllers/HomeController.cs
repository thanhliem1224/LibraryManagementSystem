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
            DateTime _dt_now = CNCFClass.GoToEndOfDay(DateTime.Now);
            ViewBag.ds_quahan = from m in db.MuonTraSach
                              where m.HanTra <= _dt_now && m.NgayTra == null
                              orderby m.HanTra ascending
                              select m;
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