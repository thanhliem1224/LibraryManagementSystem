using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using LibraryManagementSystem.Models;
using PagedList;

namespace LibraryManagementSystem.Controllers
{
    public class ThanhLyController : Controller
    {
        private CNCFContext db = new CNCFContext();
        // GET: ThanhLy
        public ActionResult Index(string maSach, string tenSach, DateTime? ngayFrom, DateTime? ngayTo, string sortOrder, int? page)
        {
            var thanhLy = db.ThanhLy.Include(t => t.Sach);



            if (maSach != null)
            {
                thanhLy = from m in thanhLy
                          where m.Sach.SachID.Contains(maSach)
                          select m;
            }
            if (tenSach != null)
            {
                thanhLy = from m in thanhLy
                          where m.Sach.TenSach.Contains(tenSach)
                          select m;
            }
            if (ngayFrom != null)// && ngayMuonTo != null)
            {
                // chỉnh lại ngày
                ngayFrom = CNCFClass.GoToBeginOfDay(ngayFrom.Value);

                thanhLy = from m in thanhLy
                          where m.Ngay >= ngayFrom
                          select m;
            }
            if (ngayTo != null)
            {
                // chỉnh lại ngày
                ngayTo = CNCFClass.GoToEndOfDay(ngayTo.Value);
                thanhLy = from m in thanhLy
                          where m.Ngay <= ngayTo
                          select m;
            }

            ViewBag.sortMaSach = "maSach_ascending";
            ViewBag.sortTenSach = "tenSach_ascending";
            ViewBag.sortChuDe = "chuDe_ascending";
            ViewBag.sortNgay = "ngay_ascending";

            switch (sortOrder)
            {
                case "maSach_ascending":
                    thanhLy = thanhLy.OrderBy(t => t.Sach.SachID);
                    ViewBag.sortMaSach = "maSach_descending";
                    break;

                case "tenSach_ascending":
                    thanhLy = thanhLy.OrderBy(t => t.Sach.TenSach);
                    ViewBag.sortTenSach = "tenSach_descending";
                    break;
                case "chuDe_ascending":
                    thanhLy = thanhLy.OrderBy(t => t.Sach.ChuDe.TenChuDe);
                    ViewBag.sortChuDe = "chuDe_descending";
                    break;
                case "ngay_ascending":
                    thanhLy = thanhLy.OrderBy(t => t.Ngay);
                    ViewBag.sortNgay = "ngay_descending";
                    break;
                ////////////////////////////////////////////////////////////
                case "maSach_descending":
                    thanhLy = thanhLy.OrderByDescending(t => t.Sach.SachID);
                    ViewBag.sortMaSach = "maSach_ascending";
                    break;
                case "tenSach_descending":
                    thanhLy = thanhLy.OrderByDescending(t => t.Sach.TenSach);
                    ViewBag.sortTenSach = "tenSach_ascending";
                    break;
                case "chuDe_descending":
                    thanhLy = thanhLy.OrderByDescending(t => t.Sach.ChuDe.TenChuDe);
                    ViewBag.sortChuDe = "chuDe_ascending";
                    break;
                case "ngay_descending":
                    thanhLy = thanhLy.OrderByDescending(t => t.Ngay);
                    ViewBag.sortNgay = "ngay_ascending";
                    break;
                default:
                    thanhLy = thanhLy.OrderByDescending(t => t.Ngay);
                    ViewBag.sortNgay = "ngay_descending";
                    break;
            }

            // lưu dữ liệu search hiện tại
            ViewBag.CurrentMaSach = maSach;
            ViewBag.CurrentTenSach = tenSach;
            ViewBag.CurrentNgayFrom = ngayFrom;
            ViewBag.CurrentNgayTo = ngayTo;
            ViewBag.CurrentSort = sortOrder;

            // setup page
            int pageSize = 50; // số dòng trong 1 trang
            int pageNumber = (page ?? 1);
            return View(thanhLy.ToPagedList(pageNumber, pageSize));
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
            if (b != null)
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
