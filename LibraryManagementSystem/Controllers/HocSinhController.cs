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
    public class HocSinhController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: Hoc_Sinh
        [Authorize]
        public ActionResult Index(string tenHS, string lopHS, string sortOrder)
        {
            ViewBag.sorTen = string.IsNullOrEmpty(sortOrder) ? "name_inc" : "";
            ViewBag.sortLop = string.IsNullOrEmpty(sortOrder) ? "lop_inc" : "";

            var hoc_sinh = from s in db.HocSinh
                           select s;

            switch (sortOrder)
            {
                case "name_inc":
                    hoc_sinh = hoc_sinh.OrderBy(s => s.TenHS);
                    break;

                case "lop_inc":
                    hoc_sinh = hoc_sinh.OrderBy(s => s.Lop);
                    break;


            }

            if (!string.IsNullOrEmpty(tenHS))
            {
                hoc_sinh = hoc_sinh.Where(s => s.TenHS.Contains(tenHS));
            }
            if (!string.IsNullOrEmpty(lopHS))
            {
                hoc_sinh = hoc_sinh.Where(s => s.Lop.Contains(lopHS));
            }

            return View(hoc_sinh);
        }

        // GET: Hoc_Sinh/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HocSinh hoc_Sinh = db.HocSinh.Find(id);
            if (hoc_Sinh == null)
            {
                return HttpNotFound();
            }
            return View(hoc_Sinh);
        }

        // GET: Hoc_Sinh/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hoc_Sinh/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TenHS,Lop,NgaySinh")] HocSinh hoc_Sinh)
        {
            if (ModelState.IsValid)
            {
                db.HocSinh.Add(hoc_Sinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hoc_Sinh);
        }

        // GET: Hoc_Sinh/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HocSinh hoc_Sinh = db.HocSinh.Find(id);
            if (hoc_Sinh == null)
            {
                return HttpNotFound();
            }
            return View(hoc_Sinh);
        }

        // POST: Hoc_Sinh/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TenHS,Lop,NgaySinh")] HocSinh hoc_Sinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoc_Sinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hoc_Sinh);
        }

        // GET: Hoc_Sinh/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HocSinh hoc_Sinh = db.HocSinh.Find(id);
            if (hoc_Sinh == null)
            {
                return HttpNotFound();
            }
            return View(hoc_Sinh);
        }

        // POST: Hoc_Sinh/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HocSinh hoc_Sinh = db.HocSinh.Find(id);
            db.HocSinh.Remove(hoc_Sinh);
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
