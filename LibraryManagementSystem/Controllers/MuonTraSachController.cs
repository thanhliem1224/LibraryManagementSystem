﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.ViewModel;
using PagedList;

namespace LibraryManagementSystem.Controllers
{
    public class MuonTraSachController : Controller
    {
        private CNCFContext db = new CNCFContext();
        private DateTime _end_day_now = CNCFClass.GoToEndOfDay(DateTime.Now);

        // GET: MuonTraSach
        public ActionResult Index(string lopHS, string tenHS, string maSach, string tenSach, DateTime? ngayFrom, DateTime? ngayTo, int? type, int? page)
        {
            var muonsach = db.MuonTraSach.Include(m => m.HocSinh).Include(m => m.Sach).Where(m => m.NgayTra == null);

            if (lopHS != null)
            {
                muonsach = from m in muonsach
                           where m.HocSinh.Lop.Contains(lopHS)
                           select m;
            }
            if (tenHS != null)
            {
                muonsach = from m in muonsach
                           where m.HocSinh.TenHS.Contains(tenHS)
                           select m;
            }
            if (maSach != null)
            {
                muonsach = from m in muonsach
                           where m.Sach.SachID.Contains(maSach)
                           select m;
            }
            if (tenSach != null)
            {
                muonsach = from m in muonsach
                           where m.Sach.TenSach.Contains(tenSach)
                           select m;
            }
            if (ngayFrom != null)// && ngayMuonTo != null)
            {
                // chỉnh lại ngày
                ngayFrom = CNCFClass.GoToBeginOfDay(ngayFrom.Value);
                muonsach = from m in muonsach
                           where m.NgayMuon >= ngayFrom
                           select m;
            }
            if (ngayTo != null)
            {
                // chỉnh lại ngày
                ngayTo = CNCFClass.GoToEndOfDay(ngayTo.Value);
                muonsach = from m in muonsach
                           where m.NgayMuon <= ngayTo
                           select m;
            }
            if (type == 1) // nếu loại là 1: mượn sách quá hạn
            {
                // muonsach.Where(m => m.HanTra <= _end_day_now);
                muonsach = from m in muonsach
                           where m.HanTra <= _end_day_now
                           select m;
                TempData["Title"] = "Danh Sách Học Sinh Mượn Sách Quá Hạn";
            }
            if (type == 0 || type == null) // nếu loại là 1: sách đang mượn
            {
                TempData["Title"] = "Danh Sách Học Sinh Đang Mượn Sách";
            }

            muonsach = from m in muonsach
                       orderby m.NgayMuon
                       select m;

            // lưu dữ liệu search hiện tại
            ViewBag.CurrentLopHS = lopHS;
            ViewBag.CurrentTenHS = tenHS;
            ViewBag.CurrentMaSach = maSach;
            ViewBag.CurrentTenSach = tenSach;
            ViewBag.CurrentNgayFrom = ngayFrom;
            ViewBag.CurrentNgayTo = ngayTo;
            ViewBag.CurrentType = type;

            // setup page
            int pageSize = 50; // số dòng trong 1 trang
            int pageNumber = (page ?? 1);

            return View(muonsach.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult LichSu(string lopHS, string tenHS, string maSach, string tenSach, DateTime? ngayFrom, DateTime? ngayTo, int? page)
        {
            var muonTraSach = db.MuonTraSach.Include(m => m.HocSinh).Include(m => m.Sach);

            if (lopHS != null)
            {
                muonTraSach = from m in muonTraSach
                              where m.HocSinh.Lop.Contains(lopHS)
                              select m;
            }
            if (tenHS != null)
            {
                muonTraSach = from m in muonTraSach
                              where m.HocSinh.TenHS.Contains(tenHS)
                              select m;
            }
            if (maSach != null)
            {
                muonTraSach = from m in muonTraSach
                              where m.Sach.SachID.Contains(maSach)
                              select m;
            }
            if (tenSach != null)
            {
                muonTraSach = from m in muonTraSach
                              where m.Sach.TenSach.Contains(tenSach)
                              select m;
            }
            if (ngayFrom != null)// && ngayMuonTo != null)
            {
                // chỉnh lại ngày
                ngayFrom = CNCFClass.GoToBeginOfDay(ngayFrom.Value);
                muonTraSach = from m in muonTraSach
                              where m.NgayMuon >= ngayFrom
                              select m;
            }
            if (ngayTo != null)
            {
                // chỉnh lại cuối ngày
                ngayTo = CNCFClass.GoToEndOfDay(ngayTo.Value);
                muonTraSach = from m in muonTraSach
                              where m.NgayMuon <= ngayTo
                              select m;
            }
            muonTraSach = from m in muonTraSach
                          orderby m.NgayMuon descending
                          select m;

            // lưu dữ liệu search hiện tại
            ViewBag.CurrentLopHS = lopHS;
            ViewBag.CurrentTenHS = tenHS;
            ViewBag.CurrentMaSach = maSach;
            ViewBag.CurrentTenSach = tenSach;
            ViewBag.CurrentNgayFrom = ngayFrom;
            ViewBag.CurrentNgayTo = ngayTo;

            // setup page
            int pageSize = 50; // số dòng trong 1 trang
            int pageNumber = (page ?? 1);

            return View(muonTraSach.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult TimHocSinh(string tenHS)
        {
            if (!string.IsNullOrEmpty(tenHS))
            {
                var hocsinh = db.HocSinh.Where(h => h.TenHS.Contains(tenHS));
                ViewBag.HocSinh = hocsinh;
            }
            return View();
        }

        public ActionResult HocSinh(int? id, string tenSach)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HocSinh hocsinh = db.HocSinh.Find(id);
            if (hocsinh == null)
            {
                return HttpNotFound();
            }

            var sachdangmuon = db.MuonTraSach.Where(m => m.HocSinhID == id).Where(m => m.NgayTra == null);
            var sachdamuon = db.MuonTraSach.Where(m => m.HocSinhID == id).Where(m => m.NgayTra != null);
            var sachmat = sachdamuon.Where(m => m.Mat == true);


            if (!string.IsNullOrEmpty(tenSach))
            {
                var dssach = db.Sach.Where(s => s.TrangThai == TrangThai.CoSan);
                dssach = dssach.Where(s => s.TenSach.Contains(tenSach));

                if (dssach.Count() > 0)// neu có kết quả tìm kiếm sách
                {
                    ViewBag.SachID = new SelectList(dssach, "ID", "IDandTen");
                }
                else
                {
                    TempData["Message"] = "Không có sách tên " + tenSach;
                }
            }

            if (sachdangmuon.Count() > 0)
            {
                ViewBag.SachDangMuon = sachdangmuon;
            }
            if (sachdamuon.Count() > 0)
            {
                ViewBag.SachDaMuon = sachdamuon;
            }
            if (sachmat.Count() > 0)
            {
                ViewBag.SachMat = sachmat;
            }
            return View(hocsinh);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "SachID,HocSinhID")] MuonTraSach muonTraSach)
        {

            if (ModelState.IsValid)
            {
                // kiểm tra số luọng sách học sinh đã mượn
                var mts = from m in db.MuonTraSach
                          where m.HocSinhID == muonTraSach.HocSinhID && m.NgayTra == null
                          select m;
                if (mts.Count() < 5) // nếu nhỏ hơn 5 thì cho mượn sách
                {
                    // kiểm tra đã quá hạn trả sách chưa
                    var mts2 = from m in mts
                               where m.HanTra < DateTime.Now
                               select m;
                    if (mts2.Count() > 0) //có sách quá hạn trả
                    {
                        // thông báo
                        TempData["Message"] = "Học Sinh Vẫn Chưa Trả Sách";
                    }
                    else //không có sách đến hạn trả, đc phếp mượn
                    {
                        // cho mượn
                        muonTraSach.NgayMuon = DateTime.Now;
                        muonTraSach.HanTra = CNCFClass.GoToEndOfDay(DateTime.Now.AddDays(7));
                        db.MuonTraSach.Add(muonTraSach);
                        db.SaveChanges();

                        // update trạng thái sach
                        Sach s = db.Sach.Single(c => c.ID == muonTraSach.SachID);
                        s.TrangThai = TrangThai.DangMuon;
                        db.SaveChanges();
                        TempData["Success"] = "Thêm thành công " + s.IDandTen;
                    }
                }
                else  // nếu đã mượn 5 cuốn sách
                {
                    // thông báo
                    TempData["Message"] = "Học Sinh Vượt Quá Số Lượng Mượn Sách";
                }
                return RedirectToAction("HocSinh", new { id = muonTraSach.HocSinhID });
            }
            else
            {
                return View("SthError");
            }
        }

        // GET: MuonTraSach/Tra Sach/5
        public ActionResult TraSach(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            return View(muonTraSach);
        }

        // POST: MuonTraSach/TraSach/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TraSach([Bind(Include = "ID, SachID")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                // update trạng thái sach
                Sach s = db.Sach.Find(muonTraSach.SachID); // tìm sách
                if (s == null) // nếu tìm không thấy sách
                {
                    return HttpNotFound();
                }
                else // nếu tìm thấy sách
                {
                    if (s.TrangThai == TrangThai.CoSan)  // nếu sách đã dc trả
                    {
                        TempData["error"] = "Sách hiện tại đang ở trong thư viện @@ !!!";
                        return View("SthError");
                    }
                    else // nếu sách chưa dc trả
                    {
                        s.TrangThai = TrangThai.CoSan; // trả sách
                        db.SaveChanges(); // lưu
                    }
                }
                // update muontrasach
                MuonTraSach mts = db.MuonTraSach.First(m => m.ID == muonTraSach.ID);
                mts.NgayTra = DateTime.Now;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(muonTraSach);
        }

        // GET: MuonTraSach/BaoMat/5
        public ActionResult BaoMat(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            return View(muonTraSach);
        }

        // POST: MuonTraSach/BaoMat/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BaoMat([Bind(Include = "ID, SachID")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                // update trạng thái sach
                Sach s = db.Sach.Single(c => c.ID == muonTraSach.SachID);
                s.TrangThai = TrangThai.Mat;
                db.SaveChanges();

                // update muontrasach
                MuonTraSach mts = db.MuonTraSach.First(m => m.ID == muonTraSach.ID);
                mts.NgayTra = DateTime.Now;
                mts.Mat = true;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "IDandTen", muonTraSach.SachID);
            return View(muonTraSach);
        }


        // GET: MuonTraSach/GiaHan/5
        public ActionResult GiaHan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            return View(muonTraSach);
        }

        // POST: MuonTraSach/GiaHan/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GiaHan([Bind(Include = "ID,HanTra")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                // update muontrasach
                MuonTraSach mts = db.MuonTraSach.First(m => m.ID == muonTraSach.ID);
                mts.HanTra = muonTraSach.HanTra;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(muonTraSach);
        }

        // GET: MuonTraSach/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", muonTraSach.SachID);
            return View(muonTraSach);
        }

        // POST: MuonTraSach/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SachID,HocSinhID,NgayMuon,HanTra,NgayTra")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                db.Entry(muonTraSach).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HocSinhID = new SelectList(db.HocSinh, "ID", "TenHS", muonTraSach.HocSinhID);
            ViewBag.SachID = new SelectList(db.Sach, "ID", "SachID", muonTraSach.SachID);
            return View(muonTraSach);
        }

        /*
        // GET: MuonTraSach/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            return View(muonTraSach);
        }
        */
        /*
        // GET: MuonTraSach/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            if (muonTraSach == null)
            {
                return HttpNotFound();
            }
            return View(muonTraSach);
        }

        // POST: MuonTraSach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MuonTraSach muonTraSach = db.MuonTraSach.Find(id);
            db.MuonTraSach.Remove(muonTraSach);
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
