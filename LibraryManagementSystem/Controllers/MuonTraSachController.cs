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
            var muonTraSach = db.MuonTraSach.Include(m => m.HocSinh).Include(m => m.Sach);
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

        // GET: MuonTraSach
        [HttpPost]
        [Authorize]
        public ActionResult TimHocSinhSach(string tenHS, string tenSach, string confirmed)
        {
            ViewBag.tenHS = tenHS;
            ViewBag.tenSach = tenSach;
            ViewBag.confirmed = confirmed;
            return RedirectToAction("Create", "MuonTraSach", new { tenHS = ViewBag.tenHS, tenSach = ViewBag.tenSach, confirmed = ViewBag.confirmed });
        }

        // GET: MuonTraSach/Create
        [Authorize]
        public ActionResult Create(string tenHS, string tenSach, string confirmed, string stt)
        {
            string confirmed1;
            string temp_tenHS = tenHS;
            string temp_tenSach = tenSach;
            var hoc_sinh = from h in db.HocSinh
                           select h;

            var sach = from s in db.Sach
                       where s.TrangThai == TrangThai.CoSan
                       select s;

            var muon_Sach = db.MuonTraSach.Include(m => m.HocSinh).Include(m => m.Sach);

            if (!string.IsNullOrEmpty(tenHS))
            {
                hoc_sinh = hoc_sinh.Where(h => h.TenHS.Contains(temp_tenHS));
                muon_Sach = muon_Sach.Where(h => h.HocSinh.TenHS.Contains(temp_tenHS)); ;

            }

            if (!string.IsNullOrEmpty(tenSach))
            {
                sach = sach.Where(s => s.TenSach.Contains(temp_tenSach));
            }


            if (confirmed == null)
            {
                confirmed1 = "no";
            }
            else
            {
                confirmed1 = confirmed;
            }

            if(stt == null)
            {
                ViewBag.stt = "OK";
            }
            else
            {
                ViewBag.stt = stt;
            }


            ViewBag.HocSinhID = new SelectList(hoc_sinh, "ID", "TenHS");
            ViewBag.SachID = new SelectList(sach, "ID", "IDandTen");
            ViewBag.confirm = confirmed1;
            ViewBag.view_muon_sach = muon_Sach;
            ViewBag.Limited = "Học sinh đã đạt giới hạn mượn 5 cuốn.";
            ViewBag.Expired = "Học sinh đã đến hạn trả sách nhưng chưa trả.";
            return View();
        }

        // POST: MuonTraSach/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SachID,HocSinhID,NgayMuon,HanTra,NgayTra")] MuonTraSach muonTraSach, string tenHS, string tenSach)
        {
            string temp_tenHS = tenHS;
            string temp_tenSach = tenSach;
            var hoc_sinh = from h in db.HocSinh
                           select h;

            var sach = from s in db.Sach
                       select s;

            if (!string.IsNullOrEmpty(tenHS))
            {
                hoc_sinh = hoc_sinh.Where(h => h.TenHS.Contains(temp_tenHS));
            }

            if (!string.IsNullOrEmpty(tenSach))
            {
                sach = sach.Where(s => s.TenSach.Contains(temp_tenSach));
            }


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
                        return RedirectToAction("Create", new { tenHS = temp_tenHS, confirmed = "yes", stt = "expired" });
                    }
                    else //không có sách đến hạn trả, đc phếp mượn
                    {
                        // update trạng thái sach
                        Sach s = db.Sach.Single(c => c.ID == muonTraSach.SachID);
                        s.TrangThai = TrangThai.DangMuon;
                        db.SaveChanges();

                        // cho mượn
                        db.MuonTraSach.Add(muonTraSach);
                        db.SaveChanges();
                        return RedirectToAction("Create", new { tenHS = temp_tenHS, confirmed = "yes" });
                        
                    }
                }
                else
                {
                    return RedirectToAction("Create", new { tenHS = temp_tenHS, confirmed = "yes", stt = "limited" });
                }
            }

            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", muonTraSach.SachID);
            return View(muonTraSach);
        }

        // GET: MuonTraSach/Edit/5
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
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "IDandTen", muonTraSach.SachID);
            return View(muonTraSach);
        }

        // POST: MuonTraSach/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TraSach([Bind(Include = "ID,SachID,HocSinhID,NgayMuon,HanTra,NgayTra")] MuonTraSach muonTraSach)
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
