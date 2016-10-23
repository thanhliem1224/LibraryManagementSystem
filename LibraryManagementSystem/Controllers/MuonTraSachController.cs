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
using LibraryManagementSystem.Models.ViewModel;
using PagedList;

namespace LibraryManagementSystem.Controllers
{
    public class MuonTraSachController : Controller
    {
        private CNCFContext db = new CNCFContext();
        private DateTime _end_day_now = CNCFClass.GoToEndOfDay(DateTime.Now);
        private DateTime _endDayYesterday = CNCFClass.GoToEndOfDay(DateTime.Now.AddDays(-1));
        private DateTime _beginDayNow = CNCFClass.GoToBeginOfDay(DateTime.Now);

        // GET: MuonTraSach
        public ActionResult Index(string lopHS, string tenHS, string maSach, string tenSach, DateTime? ngayFrom, DateTime? ngayTo, string sortOrder, string type, int? page, int? pageSize)
        {
            var muonTraSach = db.MuonTraSach.Include(m => m.HocSinh).Include(m => m.Sach).Where(m => m.NgayTra == null);
            // load dropdow lớp
            var lop = from l in db.HocSinh
                      group l by l.Lop into g
                      select g.Key;
            ViewBag.lopHS = new SelectList(lop);

            #region Search

            if (!string.IsNullOrEmpty(lopHS))
            {
                muonTraSach = from m in muonTraSach
                              where m.HocSinh.Lop.Equals(lopHS)
                              select m;
            }
            if (!string.IsNullOrEmpty(tenHS))
            {
                muonTraSach = from m in muonTraSach
                              where m.HocSinh.TenHS.Contains(tenHS)
                              select m;
            }
            if (!string.IsNullOrEmpty(maSach))
            {
                muonTraSach = from m in muonTraSach
                              where m.Sach.SachID.Contains(maSach)
                              select m;
            }
            if (!string.IsNullOrEmpty(tenSach))
            {
                muonTraSach = from m in muonTraSach
                              where m.Sach.TenSach.Contains(tenSach)
                              select m;
            }
            if (ngayFrom.HasValue)// && ngayMuonTo != null)
            {
                // chỉnh lại đầu ngày
                ngayFrom = CNCFClass.GoToBeginOfDay(ngayFrom.Value);
                muonTraSach = from m in muonTraSach
                              where m.NgayMuon >= ngayFrom
                              select m;
            }
            if (ngayTo.HasValue)
            {
                // chỉnh lại cuối ngày
                ngayTo = CNCFClass.GoToEndOfDay(ngayTo.Value);
                muonTraSach = from m in muonTraSach
                              where m.NgayMuon <= ngayTo
                              select m;
            }
            if (type == "Sách đang mượn" || type == null) // nếu loại là 0: sách đang mượn
            {
                TempData["Title"] = "Danh Sách Học Sinh Đang Mượn Sách";
            }
            if (type == "Sách cần trả hôm nay") // nếu loại là 1: sách cần trả hôm nay
            {

                muonTraSach = from m in muonTraSach
                              where m.HanTra >= _beginDayNow && m.HanTra <= _end_day_now
                              select m;

                TempData["Title"] = "Danh Sách Học Sinh Cần Trả Sách Hôm Nay";
            }
            if (type == "Sách mượn quá hạn") // nếu loại là 2: mượn sách quá hạn
            {
                // muonTraSach.Where(m => m.HanTra <= _end_day_now);
                muonTraSach = from m in muonTraSach
                              where m.HanTra <= _endDayYesterday
                              select m;
                TempData["Title"] = "Danh Sách Học Sinh Mượn Sách Quá Hạn";
            }

            #endregion
            #region Sort
            ViewBag.sortLop = "lop_ascending";
            ViewBag.sortTenHS = "tenHS_ascending";
            ViewBag.sortNgaySinh = "ngaySinh_ascending";
            ViewBag.sortTenSach = "tenSach_ascending";
            ViewBag.sortNgayMuon = "ngayMuon_ascending";
            ViewBag.sortHanTra = "hanTra_ascending";

            switch (sortOrder)
            {
                case "lop_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.HocSinh.Lop);
                    ViewBag.sortLop = "lop_descending";
                    break;
                case "tenHS_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.HocSinh.TenHS);
                    ViewBag.sortTenHS = "tenHS_descending";
                    break;
                case "ngaySinh_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.HocSinh.NgaySinh);
                    ViewBag.sortNgaySinh = "ngaySinh_descending";
                    break;
                case "tenSach_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.Sach.TenSach);
                    ViewBag.sortTenSach = "tenSach_descending";
                    break;
                case "ngayMuon_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.NgayMuon);
                    ViewBag.sortNgayMuon = "ngayMuon_descending";
                    break;
                case "hanTra_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.HanTra);
                    ViewBag.sortHanTra = "hanTra_descending";
                    break;

                //////////////////////////////////////////////////////////
                case "lop_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.HocSinh.Lop);
                    ViewBag.sortLop = "lop_ascending";
                    break;
                case "tenHS_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.HocSinh.TenHS);
                    ViewBag.sortTenHS = "tenHS_ascending";
                    break;
                case "ngaySinh_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.HocSinh.NgaySinh);
                    ViewBag.sortNgaySinh = "ngaySinh_ascending";
                    break;
                case "tenSach_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.Sach.TenSach);
                    ViewBag.sortTenSach = "tenSach_ascending";
                    break;
                case "ngayMuon_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.NgayMuon);
                    ViewBag.sortNgayMuon = "ngayMuon_ascending";
                    break;
                case "hanTra_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.HanTra);
                    ViewBag.sortHanTra = "hanTra_ascending";
                    break;
                default:
                    muonTraSach = muonTraSach.OrderByDescending(m => m.NgayMuon);
                    ViewBag.sortNgayMuon = "ngayMuon_ascending";
                    break;

            }
            #endregion
            #region Page
            // lưu dữ liệu search hiện tại
            ViewBag.CurrentLopHS = lopHS;
            ViewBag.CurrentTenHS = tenHS;
            ViewBag.CurrentMaSach = maSach;
            ViewBag.CurrentTenSach = tenSach;
            ViewBag.CurrentNgayFrom = ngayFrom;
            ViewBag.CurrentNgayTo = ngayTo;
            ViewBag.CurrentType = type;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentPageSize = pageSize;

            List<string> listtype = new List<string>() { "Sách đang mượn", "Sách cần trả hôm nay", "Sách mượn quá hạn" };
            ViewBag.type = new SelectList(listtype);

            // setup page size
            List<int> listpagesize = new List<int>() { 20, 50, 100, 150, 200 };
            ViewBag.pageSize = new SelectList(listpagesize);

            // setup page
            int thisPageSize = (pageSize ?? 20); // số dòng trong 1 trang
            int pageNumber = (page ?? 1);
            #endregion
            return View(muonTraSach.ToPagedList(pageNumber, thisPageSize));
        }
        public ActionResult LichSu(string lopHS, string tenHS, string maSach, string tenSach, DateTime? ngayFrom, DateTime? ngayTo, string sortOrder, int? page, int? pageSize)
        {
            var muonTraSach = db.MuonTraSach.Include(m => m.HocSinh).Include(m => m.Sach);
            // load dropdow lớp
            var lop = from l in db.HocSinh
                      group l by l.Lop into g
                      select g.Key;
            ViewBag.lopHS = new SelectList(lop);

            #region Tìm Kiếm
            if (lopHS != null)
            {
                muonTraSach = from m in muonTraSach where m.HocSinh.Lop.Equals(lopHS) select m;
            }
            if (tenHS != null)
            {
                muonTraSach = from m in muonTraSach where m.HocSinh.TenHS.Contains(tenHS) select m;
            }
            if (maSach != null)
            {
                muonTraSach = from m in muonTraSach where m.Sach.SachID.Contains(maSach) select m;
            }
            if (tenSach != null)
            {
                muonTraSach = from m in muonTraSach where m.Sach.TenSach.Contains(tenSach) select m;
            }
            if (ngayFrom != null)
            {
                ngayFrom = CNCFClass.GoToBeginOfDay(ngayFrom.Value);// chỉnh lại đầu ngày
                muonTraSach = from m in muonTraSach where m.NgayMuon >= ngayFrom select m;
            }
            if (ngayTo != null)
            {
                ngayTo = CNCFClass.GoToEndOfDay(ngayTo.Value);// chỉnh lại cuối ngày
                muonTraSach = from m in muonTraSach where m.NgayMuon <= ngayTo select m;
            }
            #endregion
            #region Sắp Xếp
            //khai báo ViewBag truyền sang View
            ViewBag.sortLop = "lop_ascending";
            ViewBag.sortTenHS = "tenHS_ascending";
            ViewBag.sortNgaySinh = "ngaySinh_ascending";
            ViewBag.sortTenSach = "tenSach_ascending";
            ViewBag.sortNgayMuon = "ngayMuon_ascending";
            ViewBag.sortHanTra = "hanTra_ascending";
            ViewBag.sortNgayTra = "ngayTra_ascending";

            switch (sortOrder)
            {
                //sắp xếp tăng dần theo lớp
                case "lop_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.HocSinh.Lop);
                    ViewBag.sortLop = "lop_descending"; //đổi kiểu sắp xếp giảm dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp tăng dần theo tên học sinh
                case "tenHS_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.HocSinh.TenHS);
                    ViewBag.sortTenHS = "tenHS_descending"; //đổi kiểu sắp xếp giảm dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp tăng dần theo ngày sinh
                case "ngaySinh_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.HocSinh.NgaySinh);
                    ViewBag.sortNgaySinh = "ngaySinh_descending"; //đổi kiểu sắp xếp giảm dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp tăng dần theo tên sách
                case "tenSach_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.Sach.TenSach);
                    ViewBag.sortTenSach = "tenSach_descending"; //đổi kiểu sắp xếp giảm dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp tăng dần theo ngày mượn
                case "ngayMuon_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.NgayMuon);
                    ViewBag.sortNgayMuon = "ngayMuon_descending"; //đổi kiểu sắp xếp giảm dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp tăng dần theo hạn trả
                case "hanTra_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.HanTra);
                    ViewBag.sortHanTra = "hanTra_descending"; //đổi kiểu sắp xếp giảm dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp tăng dần theo ngày trả
                case "ngayTra_ascending":
                    muonTraSach = muonTraSach.OrderBy(m => m.NgayTra);
                    ViewBag.sortNgayTra = "ngayTra_descending"; //đổi kiểu sắp xếp giảm dần cho trường hợp nhấn kế tiếp
                    break;

                //sắp xếp giảm dần theo lớp
                case "lop_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.HocSinh.Lop);
                    ViewBag.sortLop = "lop_ascending"; //đổi kiểu sắp xếp tăng dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp giảm dần theo tên học sinh
                case "tenHS_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.HocSinh.TenHS);
                    ViewBag.sortTenHS = "tenHS_ascending"; //đổi kiểu sắp xếp tăng dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp giảm dần theo ngày sinh
                case "ngaySinh_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.HocSinh.NgaySinh);
                    ViewBag.sortNgaySinh = "ngaySinh_ascending"; //đổi kiểu sắp xếp tăng dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp giảm dần theo tên sách
                case "tenSach_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.Sach.TenSach);
                    ViewBag.sortTenSach = "tenSach_ascending"; //đổi kiểu sắp xếp tăng dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp giảm dần theo ngày mượn
                case "ngayMuon_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.NgayMuon);
                    ViewBag.sortNgayMuon = "ngayMuon_ascending"; //đổi kiểu sắp xếp tăng dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp giảm dần theo hạn trả
                case "hanTra_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.HanTra);
                    ViewBag.sortHanTra = "hanTra_ascending"; //đổi kiểu sắp xếp tăng dần cho trường hợp nhấn kế tiếp
                    break;
                //sắp xếp giảm dần theo ngày trả
                case "ngayTra_descending":
                    muonTraSach = muonTraSach.OrderByDescending(m => m.NgayTra);
                    ViewBag.sortNgayTra = "ngayTra_ascending"; //đổi kiểu sắp xếp tăng dần cho trường hợp nhấn kế tiếp
                    break;
                //mặc định sắp xếp giảm dần theo ngày mượn
                default:
                    muonTraSach = muonTraSach.OrderByDescending(m => m.NgayMuon);
                    ViewBag.sortNgayMuon = "ngayMuon_ascending"; //đổi kiểu sắp xếp tăng dần cho trường hợp nhấn kế tiếp
                    break;
            }
            #endregion

            // lưu dữ liệu search hiện tại
            ViewBag.CurrentLopHS = lopHS;
            ViewBag.CurrentTenHS = tenHS;
            ViewBag.CurrentMaSach = maSach;
            ViewBag.CurrentTenSach = tenSach;
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

            return View(muonTraSach.ToPagedList(pageNumber, thisPageSize));
        }

        public ActionResult TimHocSinh(string tenHS, string lopHS)
        {
            // load dropdow lớp
            var lop = from l in db.HocSinh
                      group l by l.Lop into g
                      select g.Key;
            ViewBag.lopHS = new SelectList(lop);

            if (!string.IsNullOrEmpty(tenHS) || !string.IsNullOrEmpty(lopHS))
            {
                var ds = db.HocSinh.Select(hs => hs);
                if (!string.IsNullOrEmpty(tenHS))
                {
                    ds = db.HocSinh.Where(hs => hs.TenHS.Contains(tenHS));
                }
                if (!string.IsNullOrEmpty(lopHS))
                {
                    ds = ds.Where(s => s.Lop.Equals(lopHS));
                }
                if (ds.Count() > 0) // nếu có kết quả
                {
                    TempData["Title"] = "Kết quả tìm kiếm " + tenHS + " - " + lopHS + " (" + ds.Count() + " kết quả)";
                    ViewBag.DSTimKiem = ds;
                }
                else
                {
                    TempData["Message_Fa"] = "Không tìm thấy học sinh \"" + tenHS + "\" - " + lopHS;
                }
            }
            return View();
        }

        public ActionResult HocSinh(int? id, string tenSach, string maSach)
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

            #region Tìm kiếm sách
            // lấy danh sách sách hiện đang có sẵn trong thư viện
            var dssach = db.Sach.Where(s => s.TrangThai == TrangThai.CoSan);
            // tìm kiếm sách
            if (!string.IsNullOrEmpty(tenSach) || !string.IsNullOrEmpty(maSach))
            {
                if (!string.IsNullOrEmpty(tenSach))
                {
                    dssach = dssach.Where(s => s.TenSach.Contains(tenSach));
                }
                if (!string.IsNullOrEmpty(maSach))
                {
                    dssach = dssach.Where(s => s.SachID.Contains(maSach));
                }

                if (dssach.Count() > 0)// neu có kết quả tìm kiếm sách
                {
                    TempData["Search_result"] = "Kết quả tìm kiếm " + maSach + " - " + tenSach + " (" + dssach.Count() + " kết quả)";
                    ViewBag.SachID = new SelectList(dssach, "ID", "IDandTen");
                }
                else
                {
                    TempData["Search_result"] = "Không có sách " + maSach + " - " + tenSach;
                }
            }
            #endregion

            #region truy suất lịch sử mượn sách của học sinh
            // lấy danh sách sách đang mượn
            var sachdangmuon = db.MuonTraSach.Where(m => m.HocSinhID == id)
                .Where(m => m.NgayTra == null)
                .Where(m => m.HanTra >= _beginDayNow);
            // đưa dữ liệu sách đang mượn sang View
            if (sachdangmuon.Count() > 0)
            {
                if (sachdangmuon.Count() >= 5)
                {
                    TempData["Message"] = "Học sinh đang mượn 5 cuốn";
                }
                ViewBag.SachDangMuon = sachdangmuon;
                ViewBag.SachDangMuonCount = sachdangmuon.Count();
            }
            else
            {
                ViewBag.SachDangMuonCount = 0;
            }
            // lấy danh sách sách quá hạn
            var sachquahan = db.MuonTraSach.Where(m => m.HocSinhID == id)
                .Where(m => m.NgayTra == null).Where(m => m.HanTra <= _endDayYesterday);
            // đưa dữ liệu sách quá hạn sang View
            if (sachquahan.Count() > 0)
            {
                TempData["Message"] = "Học Sinh Vẫn Chưa Trả Sách";
                ViewBag.SachQuaHan = sachquahan;
                ViewBag.SachQuaHanCount = sachquahan.Count();
            }
            else
            {
                ViewBag.SachQuaHanCount = 0;
            }
            // lấy danh sách sách đã mượn
            var sachdamuon = db.MuonTraSach.Where(m => m.HocSinhID == id)
                .Where(m => m.NgayTra != null);
            // đưa dữ liệu sách đã mượn sang View
            if (sachdamuon.Count() > 0)
            {
                ViewBag.SachDaMuon = sachdamuon;
                ViewBag.SachDaMuonCount = sachdamuon.Count();
            }
            else
            {
                ViewBag.SachDaMuonCount = 0;
            }
            // lấy danh sách sách mất
            var sachmat = sachdamuon.Where(m => m.Mat == true);
            // đưa dữ liệu sách mất sang View
            if (sachmat.Count() > 0)
            {
                ViewBag.SachMat = sachmat;
                ViewBag.SachMatCount = sachmat.Count();
            }
            else
            {
                ViewBag.SachMatCount = 0;
            }
            #endregion
            return View(hocsinh);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "SachID,HocSinhID,NgayMuon")] MuonTraSach muonTraSach)
        {
            if (ModelState.IsValid)
            {
                //kiem tra ID học sinh có tồn tại hay không
                var _hocSinh = db.HocSinh.Find(muonTraSach.HocSinhID);
                if (_hocSinh == null)
                {
                    TempData["error"] = "Lỗi !!! Học sinh không tồn tại";
                    return View("SthError");
                }
                //kiem tra ID sách có tồn tại hay không
                var _sach = db.Sach.Find(muonTraSach.SachID);
                if (_sach == null)
                {
                    TempData["error"] = "Lỗi !!! Sách không tồn tại";
                    return View("SthError");
                }
                else  // nếu sách có tồn tại
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
                            //kiểm tra sách có trong thư viện hay không
                            if (_sach.TrangThai != TrangThai.CoSan)
                            {
                                TempData["error"] = "Lỗi !!! Sách " + _sach.TenSach + " không có trong thư viện (tình trạng: " + EnumHelper<TrangThai>.GetDisplayValue(_sach.TrangThai) + ")";
                                return View("SthError");
                            }
                            else // nếu sách có trong thư viện
                            {
                                // cho mượn
                                muonTraSach.HanTra = CNCFClass.GoToEndOfDay(muonTraSach.NgayMuon.AddDays(7));
                                db.MuonTraSach.Add(muonTraSach);
                                db.SaveChanges();

                                // update trạng thái sach
                                _sach.TrangThai = TrangThai.DangMuon;
                                db.SaveChanges();
                                TempData["Success"] = "Thêm thành công " + _sach.IDandTen;
                            }
                        }
                    }
                    else  // nếu đã mượn 5 cuốn sách
                    {
                        // thông báo
                        TempData["Message"] = "Học Sinh Vượt Quá Số Lượng Mượn Sách";
                    }
                    return RedirectToAction("HocSinh", new { id = muonTraSach.HocSinhID });
                }
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

                // lấy id học sinh trả về trang thông tin học sinh

                int id = mts.HocSinhID;
                return RedirectToAction("HocSinh", "MuonTraSach", new { id = id });
                //return RedirectToAction("Index");
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
                mts.NgayTra = DateTime.Now; // lấy ngày hiện tại của hệ thống
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
