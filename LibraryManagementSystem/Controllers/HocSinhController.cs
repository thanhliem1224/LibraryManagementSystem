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
using System.IO;
using Excel;
using System.Data.Entity.Validation;
using System.Diagnostics;
using PagedList;
using System.Globalization;

namespace LibraryManagementSystem.Controllers
{
    public class HocSinhController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: Hoc_Sinh
        [Authorize]
        public ActionResult Index(string tenHS, string lopHS, string sortOrder, int? page, int? pageSize)
        {
            var hoc_sinh = from s in db.HocSinh
                           select s;

            if (!string.IsNullOrEmpty(tenHS))
            {
                hoc_sinh = hoc_sinh.Where(s => s.TenHS.Contains(tenHS));
            }
            if (!string.IsNullOrEmpty(lopHS))
            {
                hoc_sinh = hoc_sinh.Where(s => s.Lop.Contains(lopHS));
            }
            #region Sort
            ViewBag.sortLop = "lop_ascending";
            ViewBag.sortTenHS = "tenHS_ascending";
            ViewBag.sortNgaySinh = "ngaySinh_ascending";

            switch (sortOrder)
            {
                case "lop_ascending":
                    hoc_sinh = hoc_sinh.OrderBy(h => h.Lop);
                    ViewBag.sortLop = "lop_descending";
                    break;
                case "tenHS_ascending":
                    hoc_sinh = hoc_sinh.OrderBy(h => h.TenHS);
                    ViewBag.sortTenHS = "tenHS_descending";
                    break;
                case "ngaySinh_ascending":
                    hoc_sinh = hoc_sinh.OrderBy(h => h.NgaySinh);
                    ViewBag.sortNgaySinh = "ngaySinh_descending";
                    break;
                /////////////////////////////////////////////////////////////////
                case "lop_descending":
                    hoc_sinh = hoc_sinh.OrderByDescending(h => h.Lop);
                    ViewBag.sortLop = "lop_ascending";
                    break;
                case "tenHS_descending":
                    hoc_sinh = hoc_sinh.OrderByDescending(h => h.TenHS);
                    ViewBag.sortTenHS = "tenHS_ascending";
                    break;
                case "ngaySinh_descending":
                    hoc_sinh = hoc_sinh.OrderByDescending(h => h.NgaySinh);
                    ViewBag.sortNgaySinh = "ngaySinh_ascending";
                    break;
                default:
                    hoc_sinh = hoc_sinh.OrderBy(h => h.TenHS);
                    ViewBag.sortTenHS = "tenHS_descending";
                    break;
            }

            #endregion


            // lưu dữ liệu search hiện tại
            ViewBag.CurrentLopHS = lopHS;
            ViewBag.CurrentTenHS = tenHS;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentPageSize = pageSize;

            // setup page size
            List<int> listpagesize = new List<int>() { 20, 50, 100, 150, 200 };
            ViewBag.pageSize = new SelectList(listpagesize);

            // setup page
            int thisPageSize = (pageSize ?? 20); // số dòng trong 1 trang
            int pageNumber = (page ?? 1);

            return View(hoc_sinh.ToPagedList(pageNumber, thisPageSize));
        }
        /*
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
        */
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
        public ActionResult Create([Bind(Include = "ID,MaHS,TenHS,Lop,NgaySinh")] HocSinh hoc_Sinh)
        {
            if (ModelState.IsValid)
            {
                db.HocSinh.Add(hoc_Sinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(hoc_Sinh);
        }

        public ActionResult ThemHocSinhTuFile()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ThemHocSinhTuFile(HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {

                if (upload != null && upload.ContentLength > 0)
                {
                    // ExcelDataReader works with the binary Excel file, so it needs a FileStream
                    // to get started. This is how we avoid dependencies on ACE or Interop:
                    Stream stream = upload.InputStream;

                    // We return the interface, so that
                    IExcelDataReader reader = null;


                    if (upload.FileName.EndsWith(".xls"))
                    {
                        reader = ExcelReaderFactory.CreateBinaryReader(stream);
                    }
                    else if (upload.FileName.EndsWith(".xlsx"))
                    {
                        reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    }
                    else
                    {
                        ModelState.AddModelError("File", "Định dạng file không được hỗ trợ.");
                        return View();
                    }
                    
                    reader.IsFirstRowAsColumnNames = true;

                    DataSet result = reader.AsDataSet();
                    
                    for (int i = 0; i < result.Tables.Count; i++)
                    {
                        foreach (DataRow row in result.Tables[i].Rows)
                        {
                            DateTime ngaySinh;
                            // chuyển date từ số sang date nếu là file excel 97
                            if (upload.FileName.EndsWith(".xls"))
                            {
                                double dateNumber = double.Parse(row[2].ToString());
                                ngaySinh = DateTime.FromOADate(dateNumber);
                            }
                            else // nếu là file xlsx thì giữ nguyên
                            {
                                ngaySinh = DateTime.Parse(row[2].ToString());
                            }
                            HocSinh hocsinh = new HocSinh
                            {
                                TenHS = row[1].ToString(),
                                Lop = result.Tables[i].TableName,
                                NgaySinh = ngaySinh
                            };

                            if (db.HocSinh.Any(hs => hs.NgaySinh == hocsinh.NgaySinh && hs.TenHS == hocsinh.TenHS))
                            {

                            }
                            else
                            {
                                try
                                {
                                    db.HocSinh.Add(hocsinh);
                                    db.SaveChanges();
                                }
                                catch (Exception)
                                {

                                }

                            }
                        }
                    }
                    reader.Close();
                    
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("File", "Vui lòng chọn file!");
                }

            }
            return View();
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
        public ActionResult Edit([Bind(Include = "ID,MaHS,TenHS,Lop,NgaySinh")] HocSinh hoc_Sinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoc_Sinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hoc_Sinh);
        }
        /*
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
