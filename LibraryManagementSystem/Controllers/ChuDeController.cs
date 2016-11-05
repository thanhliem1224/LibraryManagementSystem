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
using System.Threading.Tasks;
using PagedList;

namespace LibraryManagementSystem.Controllers
{
    public class ChuDeController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: Chu_De
        [Authorize]
        public ActionResult Index(string sortOrder, string chuDeSach, int? page, int? pageSize)
        {
            //lấy danhh sách chủ đề
            var chuDe = from c in db.ChuDe
                        select c;

            if (!string.IsNullOrEmpty(chuDeSach))
            {
                chuDe = chuDe.Where(c => c.TenChuDe.Contains(chuDeSach));
            }

            ViewBag.SortTen = "ten_ascending";
            switch(sortOrder)
            {
                //sắp xếp tăng dần theo tên
                case "ten_ascending":
                    chuDe = chuDe.OrderBy(c => c.TenChuDe);
                    ViewBag.SortTen = "ten_descending";
                    break;
                //sắp xếp giảm dần theo tên
                case "ten_descending":
                    chuDe = chuDe.OrderByDescending(c => c.TenChuDe);
                    ViewBag.SortTen = "ten_ascending";
                    break;
                default:
                    chuDe = chuDe.OrderBy(c => c.TenChuDe);
                    ViewBag.SortTen = "ten_descending";
                    break;
            }

            ViewBag.CurrentChuDeSach = chuDeSach;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentPageSize = pageSize;

            // setup page size
            List<int> listpagesize = new List<int>() { 20, 50, 100, 150, 200 };
            ViewBag.pageSize = new SelectList(listpagesize);

            // setup page
            int thisPageSize = (pageSize ?? 20); // số dòng trong 1 trang
            int pageNumber = (page ?? 1);

            return View(chuDe.ToPagedList(pageNumber, thisPageSize));
        }
        /*
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
        */
        // GET: Chu_De/Create
        public ActionResult Create()
        {
            return PartialView("_Create");
        }

        // POST: Chu_De/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,TenChuDe")] ChuDe chu_De)
        {
            if (ModelState.IsValid)
            {
                db.ChuDe.Add(chu_De);
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }

            return PartialView("_Create",chu_De);
        }

        // GET: Chu_De/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChuDe chu_De = await db.ChuDe.FindAsync(id);
            if (chu_De == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Edit",chu_De);
        }

        // POST: Chu_De/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,TenChuDe")] ChuDe chu_De)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chu_De).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return PartialView("_Edit",chu_De);
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
