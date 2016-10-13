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
using PagedList;

namespace LibraryManagementSystem.Controllers
{
    public class SachController : Controller
    {
        private CNCFContext db = new CNCFContext();

        // GET: Saches
        [Authorize]
        public ActionResult Index(string sachID, string tenSach, string chuDeSach, string sortOrder, int? page)
        {
            var sach = db.Sach.Include(s => s.ChuDe);

            if (!string.IsNullOrEmpty(sachID))
            {
                sach = sach.Where(s => s.SachID.Contains(sachID));
            }
            if (!string.IsNullOrEmpty(tenSach))
            {
                sach = sach.Where(s => s.TenSach.Contains(tenSach));
            }
            if (!string.IsNullOrEmpty(chuDeSach))
            {
                sach = sach.Where(x => x.ChuDe.TenChuDe == chuDeSach);
            }

            //Tìm theo chủ đề sách
            var chude_list = new List<string>();
            var chude = from l in db.ChuDe
                        orderby l.TenChuDe
                        select l.TenChuDe;
            chude_list.AddRange(chude.Distinct());

            ViewBag.chuDeSach = new SelectList(chude_list);
            //Kết thúc tìm theo chủ đề


            ViewBag.sortTenSach = "tenSach_ascending";
            ViewBag.sortMaSach = "maSach_ascending";
            ViewBag.sortChuDe = "chuDe_ascending";
            ViewBag.sortTrangThai = "trangThai_ascending";

            switch (sortOrder)
            {
                case "tenSach_ascending":
                    sach = sach.OrderBy(s => s.TenSach);
                    ViewBag.sortTenSach = "tenSach_descending";
                    break;

                case "maSach_ascending":
                    sach = sach.OrderBy(s => s.SachID);
                    ViewBag.sortMaSach = "maSach_descending";
                    break;

                case "chuDe_ascending":
                    sach = sach.OrderBy(s => s.ChuDe.ID);
                    ViewBag.sortChuDe = "chuDe_descending";
                    break;
                case "trangThai_ascending":
                    sach = sach.OrderBy(s => s.TrangThai);
                    ViewBag.sortTrangThai = "trangThai_descending";
                    break;

                ///////////////////////////////////////////////
                case "tenSach_descending":
                    sach = sach.OrderByDescending(s => s.TenSach);
                    ViewBag.sortTenSach = "tenSach_ascending";
                    break;

                case "maSach_descending":
                    sach = sach.OrderByDescending(s => s.SachID);
                    ViewBag.sortMaSach = "maSach_ascending";
                    break;

                case "chuDe_descending":
                    sach = sach.OrderByDescending(s => s.ChuDe.ID);
                    ViewBag.sortChuDe = "chuDe_ascending";
                    break;
                case "trangThai_descending":
                    sach = sach.OrderByDescending(s => s.TrangThai);
                    ViewBag.sortTrangThai = "trangThai_ascending";
                    break;
                default:
                    sach = sach.OrderBy(s => s.TenSach);
                    ViewBag.sortTenSach = "tenSach_descending";
                    break;
            }

            // lưu dữ liệu search hiện tại
            ViewBag.CurrentMaSach = sachID;
            ViewBag.CurrentTenSach = tenSach;
            ViewBag.CurrentChuDeSach = chuDeSach;
            ViewBag.CurrentSort = sortOrder;

            // setup page
            int pageSize = 50; // số dòng trong 1 trang
            int pageNumber = (page ?? 1);

            return View(sach.ToPagedList(pageNumber, pageSize));
        }


        //// GET: Saches/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Sach sach = db.Sach.Find(id);
        //    if (sach == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(sach);
        //}

        // GET: Saches/Create
        public ActionResult Create()
        {
            ViewBag.ChuDeID = new SelectList(db.ChuDe, "ID", "TenChuDe");
            return View();
        }

        // POST: Saches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ChuDeID,SachID,TenSach,SoLuong,TrangThai")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                db.Sach.Add(sach);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ChuDeID = new SelectList(db.ChuDe, "ID", "TenChuDe", sach.ChuDeID);
            return View(sach);
        }
        public ActionResult ThemSachTuFile()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ThemSachTuFile(HttpPostedFileBase upload)
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
                        // Lay ten chu de (ten sheet)
                        string tencd = result.Tables[i].TableName;

                        // them chu de vo csdl
                        if (db.ChuDe.Any(cd => cd.TenChuDe == tencd))
                        {

                        }
                        else
                        {
                            ChuDe cd = new ChuDe { TenChuDe = tencd };
                            db.ChuDe.Add(cd);
                            db.SaveChanges();
                        }

                        // them sach vo csdl

                        int idcd = db.ChuDe.Where(cd => cd.TenChuDe == tencd).Select(cd => cd.ID).FirstOrDefault(); // get ID of table Chu De by TenChuDe

                        foreach (DataRow row in result.Tables[i].Rows)
                        {
                            Sach sach = new Sach
                            {
                                ChuDeID = idcd,
                                SachID = row[0].ToString(),
                                TenSach = row[1].ToString(),
                                SoLuong = Int32.Parse(row[2].ToString()),
                                TrangThai = TrangThai.CoSan
                            };

                            if (db.Sach.Any(n => n.SachID == sach.SachID && n.TenSach == sach.TenSach))
                            {

                            }
                            else
                            {
                                db.Sach.Add(sach);
                                db.SaveChanges();
                            }
                        }
                    }

                    /*
                    foreach (DataRow row_chude in result.Tables[0].Rows)
                    {
                        foreach (DataRow row in result.Tables[row_chude["Tên chủ đề"].ToString()].Rows)
                        {
                            Sach sach = new Sach
                            {
                                ChuDeID = Int32.Parse(row_chude["ID"].ToString()),
                                SachID = row[1].ToString(),
                                TenSach = row[2].ToString(),
                                SoLuong = Int32.Parse(row[3].ToString())
                            };

                            if (db.Sach.Any(n => n.SachID == sach.SachID && n.TenSach == sach.TenSach))
                            {

                            }
                            else
                            {
                                db.Sach.Add(sach);
                                db.SaveChanges();
                            }
                        }
                    }
                    */

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

        // GET: Saches/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sach sach = db.Sach.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            ViewBag.ChuDeID = new SelectList(db.ChuDe, "ID", "TenChuDe", sach.ChuDeID);

            return View(sach);
        }

        // POST: Saches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ChuDeID,SachID,TenSach,SoLuong,TrangThai")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ChuDeID = new SelectList(db.ChuDe, "ID", "TenChuDe", sach.ChuDeID);
            return View(sach);
        }

        //// GET: Saches/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Sach sach = db.Sach.Find(id);
        //    if (sach == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(sach);
        //}

        //// POST: Saches/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Sach sach = db.Sach.Find(id);
        //    db.Sach.Remove(sach);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
