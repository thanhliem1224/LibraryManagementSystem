using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using LibraryManagementSystem.Models;
using PagedList;
using System.Collections.Generic;

namespace LibraryManagementSystem.Controllers
{
    public class DocSachController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: DocSach
        [Authorize]
        public ActionResult Index(string lopHS, string tenHS, DateTime? ngayFrom, DateTime? ngayTo, string sortOrder, int? page, int? pageSize)
        {
            var docSachTaiCho = db.DocSachTaiCho.Include(d => d.HocSinh);

            if (!string.IsNullOrEmpty(lopHS))
            {
                docSachTaiCho = from d in docSachTaiCho
                                where d.HocSinh.Lop.Contains(lopHS)
                                select d;
            }
            if (!string.IsNullOrEmpty(tenHS))
            {
                docSachTaiCho = from d in docSachTaiCho
                                where d.HocSinh.TenHS.Contains(tenHS)
                                select d;
            }
            if (ngayFrom.HasValue)
            {
                // chỉnh lại đầu ngày
                ngayFrom = CNCFClass.GoToBeginOfDay(ngayFrom.Value);
                docSachTaiCho = from d in docSachTaiCho
                                where d.Ngay >= ngayFrom
                                select d;
            }
            if (ngayTo.HasValue)
            {
                // chỉnh lại cuối ngày
                ngayTo = CNCFClass.GoToEndOfDay(ngayTo.Value);
                docSachTaiCho = from d in docSachTaiCho
                                where d.Ngay <= ngayTo
                                select d;
            }

            #region Sort
            ViewBag.sortLop = "lop_ascending";
            ViewBag.sortTenHS = "tenHS_ascending";
            ViewBag.sortNgaySinh = "ngaySinh_ascending";
            ViewBag.sortNgay = "ngay_ascending";

            switch (sortOrder)
            {
                //sắp xếp tăng dần theo lớp
                case "lop_ascending":
                    docSachTaiCho = docSachTaiCho.OrderBy(d => d.HocSinh.Lop);
                    ViewBag.sortLop = "lop_descending";
                    break;
                //sắp xếp tăng dần theo tên học sinh
                case "tenHS_ascending":
                    docSachTaiCho = docSachTaiCho.OrderBy(d => d.HocSinh.TenHS);
                    ViewBag.sortTenHS = "tenHS_descending";
                    break;
                //sắp xếp tăng dần theo ngày sinh
                case "ngaySinh_ascending":
                    docSachTaiCho = docSachTaiCho.OrderBy(d => d.HocSinh.NgaySinh);
                    ViewBag.sortNgaySinh = "ngaySinh_descending";
                    break;
                //sắp xếp tăng dần theo ngày đọc
                case "ngay_ascending":
                    docSachTaiCho = docSachTaiCho.OrderBy(d => d.Ngay);
                    ViewBag.sortNgay = "ngay_descending";
                    break;
                //sắp xếp giảm dần theo lớp
                case "lop_descending":
                    docSachTaiCho = docSachTaiCho.OrderByDescending(m => m.HocSinh.Lop);
                    ViewBag.sortLop = "lop_ascending";
                    break;
                //sắp xếp giảm dần theo tên học sinh
                case "tenHS_descending":
                    docSachTaiCho = docSachTaiCho.OrderByDescending(m => m.HocSinh.TenHS);
                    ViewBag.sortTenHS = "tenHS_ascending";
                    break;
                //sắp xếp giảm dần theo ngày sinh
                case "ngaySinh_descending":
                    docSachTaiCho = docSachTaiCho.OrderByDescending(m => m.HocSinh.NgaySinh);
                    ViewBag.sortNgaySinh = "ngaySinh_ascending";
                    break;
                //sắp xếp giảm dần theo ngày đọc
                case "ngay_descending":
                    docSachTaiCho = docSachTaiCho.OrderByDescending(m => m.Ngay);
                    ViewBag.sortNgay = "ngay_ascending";
                    break;
                //sắp xếp giảm dần theo ngày đọc
                default:
                    docSachTaiCho = docSachTaiCho.OrderByDescending(m => m.Ngay);
                    ViewBag.sortNgay = "ngay_ascending";
                    break;

            }
            #endregion
            #region phân trang
            // lưu dữ liệu search hiện tại
            ViewBag.CurrentLopHS = lopHS;
            ViewBag.CurrentTenHS = tenHS;
            ViewBag.CurrentNgayFrom = ngayFrom;
            ViewBag.CurrentNgayTo = ngayTo;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentPageSize = pageSize;

            // setup page size
            List<int> listpagesize = new List<int>() { 20, 50, 100, 150, 200 };
            ViewBag.pageSize = new SelectList(listpagesize);

            // setup page
            int thisPageSize = (pageSize ?? 20); // số dòng trong 1 trang
            int pageNumber = (page ?? 1);
            #endregion
            return View(docSachTaiCho.ToPagedList(pageNumber, thisPageSize));
        }



        [Authorize]
        public ActionResult TimHS(string tenHS)
        {
            if (!string.IsNullOrEmpty(tenHS))
            {
                var ds = db.HocSinh.Select(hs => hs);
                ds = db.HocSinh.Where(hs => hs.TenHS.Contains(tenHS));

                if (ds.Count() > 0) // nếu có kết quả
                {
                    TempData["Title"] = "Kết quả tìm kiếm \"" + tenHS + "\" (" + ds.Count() + " kết quả)";
                    ViewBag.DSTimKiem = ds;
                }
                else
                {
                    TempData["Message_Fa"] = "Không tìm thấy học sinh tên \"" + tenHS + "\"";
                }
            }

            ViewBag.DSDocSach = db.DocSachTaiCho.Where(dstc => dstc.Ngay.Day == DateTime.Now.Day
                                                        && dstc.Ngay.Month == DateTime.Now.Month
                                                        && dstc.Ngay.Year == DateTime.Now.Year)
                                                .OrderByDescending(dstc => dstc.Ngay);
            return View();
        }

        // GET: DocSach/Create
        [Authorize]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HocSinh hocSinh = db.HocSinh.Find(id);
            if (hocSinh == null)
            {
                return HttpNotFound();
            }
            return View(hocSinh);
        }

        // POST: DocSach/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,HocSinhID")] DocSachTaiCho docSachTaiCho)
        {
            if (ModelState.IsValid)
            {
                if (docSachTaiCho.HocSinhID > 0)
                {
                    var kiemtra = from d in db.DocSachTaiCho   //.Where(d => d.HocSinhID == docSachTaiCho.HocSinhID).Where(d => d.Ngay.Day == DateTime.Now.Day && d.Ngay.Month == DateTime.Now.Month && d.Ngay.Year == DateTime.Now.Year);
                                  where d.HocSinhID == docSachTaiCho.HocSinhID && (d.Ngay.Day == DateTime.Now.Day && d.Ngay.Month == DateTime.Now.Month && d.Ngay.Year == DateTime.Now.Year)
                                  select d;
                    if (kiemtra.Count() > 0)
                    {
                        string tenhs = db.HocSinh.Find(docSachTaiCho.HocSinhID).TenHS;
                        TempData["Message_Fa"] = "Học sinh " + tenhs + " đã đọc sách hôm nay.";
                        return RedirectToAction("TimHS");
                    }
                    else
                    {
                        docSachTaiCho.Ngay = DateTime.Now;
                        db.DocSachTaiCho.Add(docSachTaiCho);
                        db.SaveChanges();

                        string tenhs = db.HocSinh.Find(docSachTaiCho.HocSinhID).TenHS;
                        TempData["Message_Su"] = "Thêm thành công học sinh " + tenhs;

                        return RedirectToAction("TimHS");
                    }
                }
                else
                {
                    return View("SthError");
                }
            }

            return RedirectToAction("TimHS");
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
