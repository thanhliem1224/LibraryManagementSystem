using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class MuonTraSachController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: MuonTraSach
        public ActionResult Index()
        {
            var muontrasach = db.MuonTraSach.Where(m => m.NgayTra == null);
            if (muontrasach.Count() > 0)
            {
                ViewBag.MuonTraSach = muontrasach;
                ViewBag.MuonTraSach_Count = muontrasach.Count();
            }
            else
            {
                ViewBag.MuonTraSach_Count = 0;
            }
            return View();
        }
        public ActionResult DanhSach()
        {
            var muonTraSach = db.MuonTraSach.Include(m => m.HocSinh).Include(m => m.Sach).OrderByDescending(m => m.NgayMuon);
            return View(muonTraSach.ToList());
        }

        // GET: MuonTraSach/Details/5
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

        public ActionResult TimHocSinh(string tenHS)
        {
            if (!string.IsNullOrEmpty(tenHS))
            {
                var hocsinh = db.HocSinh.Where(h => h.TenHS.Contains(tenHS));
                ViewBag.HocSinh = hocsinh;
            }
                return View();
        }

        public ActionResult HocSinh(int? id, string tenSach)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HocSinh hocsinh = db.HocSinh.Find(id);
            if (hocsinh == null)
            {
                return HttpNotFound();
            }

            var sachdangmuon = db.MuonTraSach.Where(m => m.HocSinhID == id).Where(m => m.NgayTra == null);
            var sachdamuon = db.MuonTraSach.Where(m => m.HocSinhID == id).Where(m => m.NgayTra != null);
            var sachmat = sachdamuon.Where(m => m.Mat == true);

            var dssach = db.Sach.Where(s => s.TrangThai == TrangThai.CoSan);
            if (!string.IsNullOrEmpty(tenSach))
            {
                dssach = dssach.Where(s => s.TenSach.Contains(tenSach));
            }
            ViewBag.SachID = new SelectList(dssach, "ID", "IDandTen");

            ViewBag.SachDangMuon = sachdangmuon;
            ViewBag.SachDaMuon = sachdamuon;
            ViewBag.SachMat = sachmat;
            return View(hocsinh);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "SachID,HocSinhID")] MuonTraSach muonTraSach)
        {

            if (ModelState.IsValid)
            {
                // kiểm tra số luọng sách học sinh đã mượn
                var mts = from m in db.MuonTraSach
                          where m.HocSinhID == muonTraSach.HocSinhID && m.NgayTra == null
                          select m;
                if (mts.Count() < 5) // nếu nhỏ hơn 5 thì cho mượn sách
                {
                    // kiểm tra đã quá hạn trả sách chưa
                    var mts2 = from m in mts
                               where m.HanTra < DateTime.Now
                               select m;
                    if (mts2.Count() > 0) //có sách quá hạn trả
                    {
                        // thông báo
                        TempData["Message"] = "Học Sinh Vẫn Chưa Trả Sách";
                    }
                    else //không có sách đến hạn trả, đc phếp mượn
                    {
                        // cho mượn
                        muonTraSach.NgayMuon = DateTime.Now;
                        muonTraSach.HanTra = CNCFClass.GoToEndOfDay(DateTime.Now.AddDays(7));
                        db.MuonTraSach.Add(muonTraSach);
                        db.SaveChanges();

                        // update trạng thái sach
                        Sach s = db.Sach.Single(c => c.ID == muonTraSach.SachID);
                        s.TrangThai = TrangThai.DangMuon;
                        db.SaveChanges();

                    }
                }
                else  // nếu đã mượn 5 cuốn sách
                {
                    // thông báo
                    TempData["Message"] = "Học Sinh Vượt Quá Số Lượng Mượn Sách";
                }
                return RedirectToAction("HocSinh", new { id = muonTraSach.HocSinhID });
            }
            else
            {
                return View("SthError");
            }
        }

        // GET: MuonTraSach/Tra Sach/5
        public ActionResult TraSach(int? id)
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

        // POST: MuonTraSach/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TraSach([Bind(Include = "ID, SachID")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                // update trạng thái sach
                Sach s = db.Sach.Single(c => c.ID == muonTraSach.SachID);
                s.TrangThai = TrangThai.CoSan;
                db.SaveChanges();

                // update muontrasach
                MuonTraSach mts = db.MuonTraSach.First(m => m.ID == muonTraSach.ID);
                mts.NgayTra = DateTime.Now;
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            return View(muonTraSach);
        }

        // GET: MuonTraSach/Edit/5
        public ActionResult BaoMat(int? id)
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

        // POST: MuonTraSach/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BaoMat([Bind(Include = "ID, SachID")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                // update trạng thái sach
                Sach s = db.Sach.Single(c => c.ID == muonTraSach.SachID);
                s.TrangThai = TrangThai.Mat;
                db.SaveChanges();

                // update muontrasach
                MuonTraSach mts = db.MuonTraSach.First(m => m.ID == muonTraSach.ID);
                mts.NgayTra = DateTime.Now;
                mts.Mat = true;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "IDandTen", muonTraSach.SachID);
            return View(muonTraSach);
        }

        // GET: MuonTraSach/Edit/5
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

        // POST: MuonTraSach/Edit/5
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

        // GET: MuonTraSach/Delete/5
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

        // POST: MuonTraSach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            db.MuonTraSach.Remove(muonTraSach);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
