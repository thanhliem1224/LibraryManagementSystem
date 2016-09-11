using System.Linq;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using System;
using LibraryManagementSystem.Models.ViewModel;
using System.Web.Helpers;
using System.Collections;

namespace LibraryManagementSystem.Controllers
{
    public class BaoCaoController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: BaoCao
        [Authorize]
        public ActionResult Index(DateTime? date)
        {
            int _month = DateTime.Now.Month;
            int _year = DateTime.Now.Year;

            if (date.HasValue)
            {
                _month = date.Value.Month;
                _year = date.Value.Year;
            }

            // muon tra sach trong thang 
            var muontrasach = from m in db.MuonTraSach
                              where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year
                              group m by new { m.HocSinh.Lop, m.HocSinh.TenHS } into g
                              select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupSoLuong = g.Count() };

            if (muontrasach.Count() > 0)
            {
                ViewBag.MuonTraSach_Count = muontrasach.Sum(m => m.GroupSoLuong);
                ViewBag.MuonTraSach = muontrasach;
            }
            else
            {
                ViewBag.MuonTraSach_Count = 0;
            }

            // muon sach chua tra
            var muonchuatra = from m in db.MuonTraSach
                              where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year && m.NgayTra == null
                              group m by m.HocSinh.TenHS into g
                              select new BaoCaoSachChuaTra { TenHS = g.Key, SoLuong = g.Count(), DanhSachMuon = g };

            if (muonchuatra.Count() > 0)
            {
                ViewBag.MuonChuaTra = muonchuatra;
                ViewBag.MuonChuaTra_Count = muonchuatra.Sum(m => m.SoLuong);
            }
            else
            {
                ViewBag.MuonChuaTra_Count = 0;
            }

            // muon sach qua han
            var muonquahan = from m in db.MuonTraSach
                              where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year && m.NgayTra == null && m.HanTra < DateTime.Now
                              group m by m.HocSinh.TenHS into g
                              select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count(), GroupDanhSach = g };

            if (muonquahan.Count() > 0)
            {
                ViewBag.MuonQuaHan = muonquahan;
                ViewBag.MuonQuaHan_Count = muonquahan.Sum(m => m.GroupSoLuong);
            }
            else
            {
                ViewBag.MuonQuaHan_Count = 0;
            }

            // thong ke sach
            var thongkesach = from s in db.Sach
                              group s by s.ChuDe.TenChuDe into g
                              select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count() };
            if (thongkesach.Count() > 0)
            {
                ViewBag.ThongKeSach = thongkesach;
                ViewBag.ThongKeSach_Count = thongkesach.Sum(t => t.GroupSoLuong);
            }
            else
            {
                ViewBag.ThongKeSach_Count = 0;
            }

            ViewBag.date = _month + "/" + _year;





            return View();
        }


        /*
        // GET: BaoCao/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            return View(muonTraSach);
        }

        // GET: BaoCao/Create
        public ActionResult Create()
        {
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS");
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID");
            return View();
        }

        // POST: BaoCao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SachID,HocSinhID,NgayMuon,HanTra,NgayTra")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                db.MuonTraSach.Add(muonTraSach);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", muonTraSach.SachID);
            return View(muonTraSach);
        }

        // GET: BaoCao/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", muonTraSach.SachID);
            return View(muonTraSach);
        }

        // POST: BaoCao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SachID,HocSinhID,NgayMuon,HanTra,NgayTra")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                db.Entry(muonTraSach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", muonTraSach.SachID);
            return View(muonTraSach);
        }

        // GET: BaoCao/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            return View(muonTraSach);
        }

        // POST: BaoCao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            db.MuonTraSach.Remove(muonTraSach);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        */

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
