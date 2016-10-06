using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class ThanhLyController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: ThanhLy
        public ActionResult Index()
        {
            var thanhLy = db.ThanhLy.Include(t => t.Sach);
            return View(thanhLy.ToList());
        }
        /*
        // GET: ThanhLy/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhLy thanhLy = db.ThanhLy.Find(id);
            if (thanhLy == null)
            {
                return HttpNotFound();
            }
            return View(thanhLy);
        }
        */
        [HttpPost]
        [Authorize]
        public ActionResult TimSach(string tenSach)
        {
            return RedirectToAction("Create", "ThanhLy", new { b = tenSach });
        }

        // GET: ThanhLy/Create
        public ActionResult Create(string b)
        {
            if(b != null)
            {
                var ds = db.Sach.Where(s => s.TrangThai == TrangThai.CoSan);
                ds = ds.Where(s => s.TenSach.Contains(b));

                if (ds.Count() > 0)
                {
                    ViewBag.SachID = new SelectList(ds, "ID", "IDandTen");
                }
                else
                {
                    TempData["Message_Fa"] = "Không có sách tên " + b;
                }

                
            }

            return View();
        }

        // POST: ThanhLy/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SachID")] ThanhLy thanhLy)
        {
            if (ModelState.IsValid)
            {
                // update trạng thái sach
                Sach s = db.Sach.Find(thanhLy.SachID); // tim sach

                if (s == null)
                {
                    return HttpNotFound();  // bao ko tim thay sach
                }
                else
                {
                    s.TrangThai = TrangThai.ThanhLy;
                    db.SaveChanges();
                }
                // update thanh ly
                thanhLy.Ngay = DateTime.Now;
                db.ThanhLy.Add(thanhLy);
                db.SaveChanges();
                // thông báo
                TempData["Message_Su"] = "Đã Thanh Lý " + s.IDandTen;
                return RedirectToAction("Create");
            }

            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", thanhLy.SachID);
            return View(thanhLy);
        }

        // GET: ThanhLy/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhLy thanhLy = db.ThanhLy.Find(id);
            if (thanhLy == null)
            {
                return HttpNotFound();
            }
            ViewBag.SachID = new SelectList(db.Sach, "ID", "IDandTen", thanhLy.SachID);
            return View(thanhLy);
        }

        // POST: ThanhLy/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SachID,Ngay")] ThanhLy thanhLy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thanhLy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", thanhLy.SachID);
            return View(thanhLy);
        }
        /*
        // GET: ThanhLy/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThanhLy thanhLy = db.ThanhLy.Find(id);
            if (thanhLy == null)
            {
                return HttpNotFound();
            }
            return View(thanhLy);
        }

        // POST: ThanhLy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThanhLy thanhLy = db.ThanhLy.Find(id);
            db.ThanhLy.Remove(thanhLy);
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
