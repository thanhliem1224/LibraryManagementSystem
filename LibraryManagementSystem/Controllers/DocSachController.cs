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
    public class DocSachController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: DocSach
        [Authorize]
        public ActionResult Index()
        {
            var docSachTaiCho = db.DocSachTaiCho.Include(d => d.HocSinh).OrderByDescending(d => d.Ngay);
            return View(docSachTaiCho.ToList());
        }

        

        [HttpPost]
        [Authorize]
        public ActionResult TimHS(string tenHS)
        {
            return RedirectToAction("Create", "DocSach", new { s = tenHS });
        }

        // GET: DocSach/Create
        [Authorize]
        public ActionResult Create(string s)
        {

            if (s != null)
            {
                var ds = db.HocSinh.Select(hs => hs);
                ds = db.HocSinh.Where(hs => hs.TenHS.Contains(s));
                
                if (ds.Count() > 0) // nếu có kết quả
                {
                    ViewBag.HocSinhID = new SelectList(ds, "ID", "TenHS");
                }
                else 
                {
                    TempData["Message_Fa"] = "Không tìm thấy học sinh tên \"" + s + "\"";
                }
            }
            

            ViewBag.DSDocSach = db.DocSachTaiCho.Where(dstc => dstc.Ngay.Day == DateTime.Now.Day && dstc.Ngay.Month == DateTime.Now.Month && dstc.Ngay.Year == DateTime.Now.Year).OrderByDescending(dstc => dstc.Ngay);
            return View();
        }

        // POST: DocSach/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,HocSinhID,TenHS")] DocSachTaiCho docSachTaiCho)
        {
            if (ModelState.IsValid)
            {
                if (docSachTaiCho.HocSinhID > 0)
                {
                    var kiemtra = from d in db.DocSachTaiCho   //.Where(d => d.HocSinhID == docSachTaiCho.HocSinhID).Where(d => d.Ngay.Day == DateTime.Now.Day && d.Ngay.Month == DateTime.Now.Month && d.Ngay.Year == DateTime.Now.Year);
                                  where d.HocSinhID == docSachTaiCho.HocSinhID && (d.Ngay.Day == DateTime.Now.Day && d.Ngay.Month == DateTime.Now.Month && d.Ngay.Year == DateTime.Now.Year)
                                  select d;
                    if (kiemtra.Count()>0)
                    {
                        string tenhs = db.HocSinh.Find(docSachTaiCho.HocSinhID).TenHS;
                        TempData["Message_Fa"] = "Học Sinh " + tenhs + " đã đọc sách hôm nay.";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        docSachTaiCho.Ngay = DateTime.Now;
                        db.DocSachTaiCho.Add(docSachTaiCho);
                        db.SaveChanges();

                        string tenhs = db.HocSinh.Find(docSachTaiCho.HocSinhID).TenHS;
                        TempData["Message_Su"] = "Thêm Thành Công " + tenhs;

                        return RedirectToAction("Create");
                    }
                }
                else
                {
                    return View("SthError");
                }
            }

            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", docSachTaiCho.HocSinhID);
            return View(docSachTaiCho);
        }
        
        // GET: DocSach/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocSachTaiCho docSachTaiCho = db.DocSachTaiCho.Find(id);
            if (docSachTaiCho == null)
            {
                return HttpNotFound();
            }
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", docSachTaiCho.HocSinhID);
            return View(docSachTaiCho);
        }

        // POST: DocSach/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,HocSinhID,Ngay")] DocSachTaiCho docSachTaiCho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(docSachTaiCho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", docSachTaiCho.HocSinhID);
            return View(docSachTaiCho);
        }
        /*
        // GET: DocSach/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocSachTaiCho docSachTaiCho = db.DocSachTaiCho.Find(id);
            if (docSachTaiCho == null)
            {
                return HttpNotFound();
            }
            return View(docSachTaiCho);
        }

        // POST: DocSach/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DocSachTaiCho docSachTaiCho = db.DocSachTaiCho.Find(id);
            db.DocSachTaiCho.Remove(docSachTaiCho);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: DocSach/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DocSachTaiCho docSachTaiCho = db.DocSachTaiCho.Find(id);
            if (docSachTaiCho == null)
            {
                return HttpNotFound();
            }
            return View(docSachTaiCho);
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
