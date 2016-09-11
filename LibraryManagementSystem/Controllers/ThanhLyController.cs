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
    public class ThanhLyController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: ThanhLy
        public ActionResult Index()
        {
            var thanhLy = db.ThanhLy.Include(t => t.Sach);
            return View(thanhLy.ToList());
        }

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

        // GET: ThanhLy/Create
        public ActionResult Create()
        {
            ViewBag.SachID = new SelectList(db.Sach.Where(s => s.TrangThai == TrangThai.CoSan) , "ID", "IDandTen");
            //ViewBag.SachID = from s in db.Sach
            //                 where s.TrangThai == TrangThai.CoSan
            //                 select s;
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
                Sach s = db.Sach.Single(c => c.ID == thanhLy.SachID);
                s.TrangThai = TrangThai.ThanhLy;
                db.SaveChanges();

                // update thanh ly
                thanhLy.Ngay = DateTime.Now;
                db.ThanhLy.Add(thanhLy);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", thanhLy.SachID);
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
