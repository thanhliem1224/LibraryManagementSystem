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
                            HocSinh hocsinh = new HocSinh
                            {
                                MaHS = row[0].ToString(),
                                TenHS = row[1].ToString(),
                                Lop = result.Tables[i].TableName,
                                NgaySinh = DateTime.Parse(row[2].ToString())
                            };

                            if (db.HocSinh.Any(hs => hs.MaHS == hocsinh.MaHS && hs.TenHS == hocsinh.TenHS))
                            {

                            }
                            else
                            {
                                try
                                {
                                    db.HocSinh.Add(hocsinh);
                                    db.SaveChanges();
                                }
                                catch (DbEntityValidationException dbEx)
                                {
                                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                                    {
                                        foreach (var validationError in validationErrors.ValidationErrors)
                                        {
                                            Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                        }
                                    }
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
