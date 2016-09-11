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
    public class ChuDeController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: Chu_De
        [Authorize]
        public ActionResult Index()
        {
            return View(db.ChuDe.ToList());
        }

        // GET: Chu_De/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChuDe chu_De = db.ChuDe.Find(id);
            if (chu_De == null)
            {
                return HttpNotFound();
            }
            return View(chu_De);
        }

        // GET: Chu_De/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Chu_De/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,TenChuDe")] ChuDe chu_De)
        {
            if (ModelState.IsValid)
            {
                db.ChuDe.Add(chu_De);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(chu_De);
        }

        // GET: Chu_De/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChuDe chu_De = db.ChuDe.Find(id);
            if (chu_De == null)
            {
                return HttpNotFound();
            }
            return View(chu_De);
        }

        // POST: Chu_De/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TenChuDe")] ChuDe chu_De)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chu_De).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chu_De);
        }

        // GET: Chu_De/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChuDe chu_De = db.ChuDe.Find(id);
            if (chu_De == null)
            {
                return HttpNotFound();
            }
            return View(chu_De);
        }

        // POST: Chu_De/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChuDe chu_De = db.ChuDe.Find(id);
            db.ChuDe.Remove(chu_De);
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
