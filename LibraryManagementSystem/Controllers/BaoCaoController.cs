using System.Linq;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using System;
using LibraryManagementSystem.Models.ViewModel;

namespace LibraryManagementSystem.Controllers
{
    public class BaoCaoController : Controller
    {
        public enum LOAIBAOCAO
        {
            Thang,
            Nam,
            KhoanThoiGian
        }

        private CNCFContext db = new CNCFContext();

        private DateTime _end_day_now = CNCFClass.GoToEndOfDay(DateTime.Now);

        [HttpPost]
        [Authorize]
        public ActionResult LuaChon(LOAIBAOCAO type, DateTime? date, DateTime? date_finish, bool? thanhly_status, bool? muonsach_status, bool? docsach_status, bool? theloaiyeuthich_status, bool? sachyeuthich_status, bool? sachmat_status, bool? muonsachquahan_status, bool? sachthuvien_status)
        {
            return RedirectToAction("Index", "BaoCao", new { type = type, d = date, d_f = date_finish, tl = thanhly_status, ms = muonsach_status, ds = docsach_status, tlyt = theloaiyeuthich_status, syt = sachyeuthich_status, sm = sachmat_status, msqh = muonsachquahan_status, stv = sachthuvien_status });
        }

        // GET: BaoCao
        [Authorize]
        public ActionResult Index(LOAIBAOCAO? type, DateTime? d, DateTime? d_f, bool? tl, bool? ms, bool? ds, bool? tlyt, bool? sm, bool? syt, bool? msqh, bool? stv)
        {
            if (type.HasValue)
            {
                if (d.HasValue || (d.HasValue && d_f.HasValue)) // check du lieu ngay
                {
                    // get date
                    if (type == LOAIBAOCAO.Thang)
                    {
                        ViewBag.date = d.Value.Month + "/" + d.Value.Year;
                    }
                    if (type == LOAIBAOCAO.Nam)
                    {
                        ViewBag.date = d.Value.Year.ToString();
                    }
                    if (type == LOAIBAOCAO.KhoanThoiGian)
                    {
                        ViewBag.date = d.Value.Month + "/" + d.Value.Year + " - " + d_f.Value.Month + "/" + d_f.Value.Year;
                    }

                    // Báo Cáo Mượn sách
                    if (ms.HasValue)
                    {
                        ViewBag.muonsach_stt = ms;
                        if (ms.Value)
                        {
                            BaoCaoMuonSach(type, d, d_f);
                        }
                    }
                    else
                    {
                        ViewBag.muonsach_stt = false;
                    }

                    // Báo Cáo Đọc sách
                    if (ms.HasValue)
                    {
                        ViewBag.docsach_stt = ms;
                        if (ms.Value)
                        {
                            BaoCaoDocSach(type, d, d_f);
                        }
                    }
                    else
                    {
                        ViewBag.muonsach_stt = false;
                    }

                    // Báo Cáo Thể Loại Sách Yêu Thích
                    if (tlyt.HasValue)
                    {
                        ViewBag.theloaiyeuthich_stt = tlyt;
                        if (tlyt.Value)
                        {
                            BaoCaoTheLoaiYeuThich(type, d, d_f);
                        }
                    }
                    else
                    {
                        ViewBag.muonsach_stt = false;
                    }

                    // Báo Cáo SÁch yêu thích
                    if (syt.HasValue)
                    {
                        ViewBag.sachyeuthich_stt = syt;
                        if (syt.Value)
                        {
                            BaoCaoSachYeuThich(type, d, d_f);
                        }
                    }
                    else
                    {
                        ViewBag.sachyeuthich_stt = false;
                    }


                    // Báo Cáo Mượn sách quá hạn
                    if (msqh.HasValue)
                    {
                        ViewBag.muonsachquahan_stt = msqh;
                        if (msqh.Value)
                        {
                            BaoCaoMuonSachQuaHan();
                        }
                    }
                    else
                    {
                        ViewBag.muonsach_stt = false;
                    }

                    // Báo Cáo Số lượng sách trong thư viện theo the loai
                    if (stv.HasValue)
                    {
                        ViewBag.sachtrongthuvien_stt = stv;
                        if (stv.Value)
                        {
                            BaoCaoSachTrongThuVien();
                        }
                    }
                    else
                    {
                        ViewBag.muonsach_stt = false;
                    }

                    // Báo cáo Thanh lý
                    if (tl.HasValue)
                    {
                        ViewBag.thanhly_stt = tl;
                        if (tl.Value)
                        {
                            BaoCaoThanhLy(type, d, d_f);
                        }
                    }
                    else
                    {
                        ViewBag.thanhly_stt = false;
                    }

                    // Báo cáo sách mất
                    if (sm.HasValue)
                    {
                        ViewBag.sachmat_stt = sm;

                        if (sm.Value)
                        {
                            var sachmat = BaoCaoSachMat();

                            if (sachmat.Count() > 0)
                            {
                                ViewBag.SachMat_Count = sachmat.Sum(m => m.GroupSoLuong);
                                ViewBag.SachMat = sachmat;
                            }
                            else
                            {
                                ViewBag.SachMat_Count = 0;
                            }
                        }
                    }
                    else
                    {
                        ViewBag.sachmat_stt = false;
                    }

                    return View();
                }
                else
                {
                    return View("SthError");
                }
            }
            else
            {
                return View("~/Views/BaoCao/LuaChon.cshtml");
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private ActionResult BaoCaoDocSach(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
        {
            if (type.HasValue)
            {
                if (type == LOAIBAOCAO.Thang)
                {
                    if (d.HasValue) // neu du lieu thang co
                    {
                        int _month = d.Value.Month;
                        int _year = d.Value.Year;

                        var danhsachdoc = from dsd in db.DocSachTaiCho
                                          where dsd.Ngay.Month == _month && dsd.Ngay.Year == _year
                                          group dsd by dsd.HocSinh into g
                                          orderby g.Key.Lop ascending, g.Key.TenHS ascending
                                          select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };
                        if (danhsachdoc.Count() > 0)
                        {
                            ViewBag.DocSachTaiCho_Count = danhsachdoc.Sum(m => m.GroupSoLuong);
                            ViewBag.DocSachTaiCho = danhsachdoc;
                        }
                        else
                        {
                            ViewBag.DocSachTaiCho_Count = 0;
                        }

                    }
                    else // neu ko co du lieu thang
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.Nam)
                {
                    if (d.HasValue) // neu co du lieu nam
                    {
                        int _year = d.Value.Year;

                        var danhsachdoc = from dsd in db.DocSachTaiCho
                                          where dsd.Ngay.Year == _year
                                          group dsd by dsd.HocSinh into g
                                          orderby g.Key.Lop ascending, g.Key.TenHS ascending
                                          select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };
                        if (danhsachdoc.Count() > 0)
                        {
                            ViewBag.DocSachTaiCho_Count = danhsachdoc.Sum(m => m.GroupSoLuong);
                            ViewBag.DocSachTaiCho = danhsachdoc;
                        }
                        else
                        {
                            ViewBag.DocSachTaiCho_Count = 0;
                        }
                    }
                    else // neu ko co du lieu nam
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.KhoanThoiGian)
                {
                    if ((d.HasValue) && (d_f.HasValue)) // neu co du lieu khoang thoi gian
                    {
                        int _month_s = d.Value.Month;
                        int _year_s = d.Value.Year;
                        int _month_f = d_f.Value.Month;
                        int _year_f = d_f.Value.Year;
                        var danhsachdoc = from dsd in db.DocSachTaiCho
                                          where (dsd.Ngay.Month >= _month_s && dsd.Ngay.Year >= _year_s) && (dsd.Ngay.Month <= _month_f && dsd.Ngay.Year <= _year_f)
                                          group dsd by dsd.HocSinh into g
                                          orderby g.Key.Lop ascending, g.Key.TenHS ascending
                                          select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };
                        if (danhsachdoc.Count() > 0)
                        {
                            ViewBag.DocSachTaiCho_Count = danhsachdoc.Sum(m => m.GroupSoLuong);
                            ViewBag.DocSachTaiCho = danhsachdoc;
                        }
                        else
                        {
                            ViewBag.DocSachTaiCho_Count = 0;
                        }
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return View("SthError");
                    }
                }
                else
                {
                    return View("SthError");
                }

            }
            else
            {
                return View("SthError");
            }

            return View();
        }

        private ActionResult BaoCaoMuonSach(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
        {
            if (type.HasValue)
            {
                if (type == LOAIBAOCAO.Thang)
                {
                    if (d.HasValue) // neu du lieu thang co
                    {
                        int _month = d.Value.Month;
                        int _year = d.Value.Year;

                        var danhsachmuonsach = from dsms in db.MuonTraSach
                                               where dsms.NgayMuon.Month == _month && dsms.NgayMuon.Year == _year
                                               group dsms by dsms.HocSinh into g
                                               orderby g.Key.Lop ascending, g.Key.TenHS ascending
                                               select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };
                        if (danhsachmuonsach.Count() > 0)
                        {
                            ViewBag.MuonTraSach_Count = danhsachmuonsach.Sum(m => m.GroupSoLuong);
                            ViewBag.MuonTraSach = danhsachmuonsach;
                        }
                        else
                        {
                            ViewBag.MuonTraSach_Count = 0;
                        }
                    }
                    else // neu ko co du lieu thang
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.Nam)
                {
                    if (d.HasValue) // neu co du lieu nam
                    {
                        int _year = d.Value.Year;

                        var danhsachmuonsach = from dsms in db.MuonTraSach
                                               where dsms.NgayMuon.Year == _year
                                               group dsms by dsms.HocSinh into g
                                               orderby g.Key.Lop ascending, g.Key.TenHS ascending
                                               select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };
                        if (danhsachmuonsach.Count() > 0)
                        {
                            ViewBag.MuonTraSach_Count = danhsachmuonsach.Sum(m => m.GroupSoLuong);
                            ViewBag.MuonTraSach = danhsachmuonsach;
                        }
                        else
                        {
                            ViewBag.MuonTraSach_Count = 0;
                        }
                    }
                    else // neu ko co du lieu nam
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.KhoanThoiGian)
                {
                    if ((d.HasValue) && (d_f.HasValue)) // neu co du lieu khoang thoi gian
                    {
                        int _month_s = d.Value.Month;
                        int _year_s = d.Value.Year;
                        int _month_f = d_f.Value.Month;
                        int _year_f = d_f.Value.Year;
                        var danhsachmuonsach = from dsms in db.MuonTraSach
                                               where (dsms.NgayMuon.Month >= _month_s && dsms.NgayMuon.Year >= _year_s) && (dsms.NgayMuon.Month <= _month_f && dsms.NgayMuon.Year <= _year_f)
                                               group dsms by dsms.HocSinh into g
                                               orderby g.Key.Lop ascending, g.Key.TenHS ascending
                                               select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };
                        if (danhsachmuonsach.Count() > 0)
                        {
                            ViewBag.MuonTraSach_Count = danhsachmuonsach.Sum(m => m.GroupSoLuong);
                            ViewBag.MuonTraSach = danhsachmuonsach;
                        }
                        else
                        {
                            ViewBag.MuonTraSach_Count = 0;
                        }
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return View("SthError");
                    }
                }
                else
                {
                    return View("SthError");
                }

            }
            else
            {
                return View("SthError");
            }

            return View();
        }

        private ActionResult BaoCaoTheLoaiYeuThich(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
        {
            if (type.HasValue)
            {
                if (type == LOAIBAOCAO.Thang)
                {
                    if (d.HasValue) // neu du lieu thang co
                    {
                        int _month = d.Value.Month;
                        int _year = d.Value.Year;

                        //The loai dc yeu thich
                        var theloaiyeuthich = from m in db.MuonTraSach
                                              where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year
                                              group m by m.Sach.ChuDe into g
                                              select new BaoCaoVM { GroupName1 = g.Key.TenChuDe, GroupSoLuong = g.Count() };

                        if (theloaiyeuthich.Count() > 0)
                        {
                            ViewBag.TheLoaiYeuThich_Count = theloaiyeuthich.Sum(m => m.GroupSoLuong);
                            ViewBag.TheLoaiYeuThich = theloaiyeuthich;
                        }
                        else
                        {
                            ViewBag.TheLoaiYeuThich_Count = 0;
                        }
                    }
                    else // neu ko co du lieu thang
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.Nam)
                {
                    if (d.HasValue) // neu co du lieu nam
                    {
                        int _year = d.Value.Year;

                        var theloaiyeuthich = from m in db.MuonTraSach
                                              where m.NgayMuon.Year == _year
                                              group m by m.Sach.ChuDe into g
                                              select new BaoCaoVM { GroupName1 = g.Key.TenChuDe, GroupSoLuong = g.Count() };

                        if (theloaiyeuthich.Count() > 0)
                        {
                            ViewBag.TheLoaiYeuThich_Count = theloaiyeuthich.Sum(m => m.GroupSoLuong);
                            ViewBag.TheLoaiYeuThich = theloaiyeuthich;
                        }
                        else
                        {
                            ViewBag.TheLoaiYeuThich_Count = 0;
                        }
                    }
                    else // neu ko co du lieu nam
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.KhoanThoiGian)
                {
                    if ((d.HasValue) && (d_f.HasValue)) // neu co du lieu khoang thoi gian
                    {
                        int _month_s = d.Value.Month;
                        int _year_s = d.Value.Year;
                        int _month_f = d_f.Value.Month;
                        int _year_f = d_f.Value.Year;
                        var theloaiyeuthich = from m in db.MuonTraSach
                                              where (m.NgayMuon.Month >= _month_s && m.NgayMuon.Year >= _year_s) && (m.NgayMuon.Month <= _month_f && m.NgayMuon.Year <= _year_f)
                                              group m by m.Sach.ChuDe into g
                                              select new BaoCaoVM { GroupName1 = g.Key.TenChuDe, GroupSoLuong = g.Count() };

                        if (theloaiyeuthich.Count() > 0)
                        {
                            ViewBag.TheLoaiYeuThich_Count = theloaiyeuthich.Sum(m => m.GroupSoLuong);
                            ViewBag.TheLoaiYeuThich = theloaiyeuthich;
                        }
                        else
                        {
                            ViewBag.TheLoaiYeuThich_Count = 0;
                        }
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return View("SthError");
                    }
                }
                else // type ko xác định
                {
                    return View("SthError");
                }

            }
            else // type ko có dữ liệu
            {
                return View("SthError");
            }

            return View();
        }

        private ActionResult BaoCaoSachYeuThich(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
        {
            if (type.HasValue)
            {
                if (type == LOAIBAOCAO.Thang)
                {
                    if (d.HasValue) // neu du lieu thang co
                    {
                        int _month = d.Value.Month;
                        int _year = d.Value.Year;

                        /// Sach yeu thich trong thang
                        var sachyeuthich = db.MuonTraSach
                            .Where(mts => mts.NgayMuon.Month == _month && mts.NgayMuon.Year == _year)
                            .GroupBy(m => m.Sach)
                            .GroupBy(m2 => m2.Key.ChuDe)
                            .SelectMany(g => g.OrderBy(row => row.Count()).Take(5))
                            .Select(result => new BaoCaoVM
                            {
                                GroupName1 = result.Key.ChuDe.TenChuDe,
                                GroupName2 = result.Key.TenSach,
                                GroupSoLuong = result.Count()
                            });

                        if (sachyeuthich.Count() > 0)
                        {
                            ViewBag.SachYeuThich_Count = sachyeuthich.Sum(m => m.GroupSoLuong);
                            ViewBag.SachYeuThich = sachyeuthich;
                        }
                        else
                        {
                            ViewBag.SachYeuThich_Count = 0;
                        }
                    }
                    else // neu ko co du lieu thang
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.Nam)
                {
                    if (d.HasValue) // neu co du lieu nam
                    {
                        int _year = d.Value.Year;


                        /// Sach yeu thich trong nam
                        var sachyeuthich = db.MuonTraSach
                            .Where(mts => mts.NgayMuon.Year == _year)
                            .GroupBy(m => m.Sach)
                            .GroupBy(m2 => m2.Key.ChuDe)
                            .SelectMany(g => g.OrderBy(row => row.Count()).Take(5))
                            .Select(result => new BaoCaoVM
                            {
                                GroupName1 = result.Key.ChuDe.TenChuDe,
                                GroupName2 = result.Key.TenSach,
                                GroupSoLuong = result.Count()
                            });

                        if (sachyeuthich.Count() > 0)
                        {
                            ViewBag.SachYeuThich_Count = sachyeuthich.Sum(m => m.GroupSoLuong);
                            ViewBag.SachYeuThich = sachyeuthich;
                        }
                        else
                        {
                            ViewBag.SachYeuThich_Count = 0;
                        }
                    }
                    else // neu ko co du lieu nam
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.KhoanThoiGian)
                {
                    if ((d.HasValue) && (d_f.HasValue)) // neu co du lieu khoang thoi gian
                    {
                        int _month_s = d.Value.Month;
                        int _year_s = d.Value.Year;
                        int _month_f = d_f.Value.Month;
                        int _year_f = d_f.Value.Year;


                        /// Sach yeu thich trong thang
                        var sachyeuthich = db.MuonTraSach
                            .Where(mts => mts.NgayMuon.Month >= _month_s && mts.NgayMuon.Year >= _year_s)
                            .Where(mts => mts.NgayMuon.Month <= _month_f && mts.NgayMuon.Year <= _year_f)
                            .GroupBy(m => m.Sach)
                            .GroupBy(m2 => m2.Key.ChuDe)
                            .SelectMany(g => g.OrderBy(row => row.Count()).Take(5))
                            .Select(result => new BaoCaoVM
                            {
                                GroupName1 = result.Key.ChuDe.TenChuDe,
                                GroupName2 = result.Key.TenSach,
                                GroupSoLuong = result.Count()
                            });

                        if (sachyeuthich.Count() > 0)
                        {
                            ViewBag.SachYeuThich_Count = sachyeuthich.Sum(m => m.GroupSoLuong);
                            ViewBag.SachYeuThich = sachyeuthich;
                        }
                        else
                        {
                            ViewBag.SachYeuThich_Count = 0;
                        }

                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return View("SthError");
                    }
                }
                else // type ko xác định
                {
                    return View("SthError");
                }

            }
            else // type ko có dữ liệu
            {
                return View("SthError");
            }

            return View();
        }

        private ActionResult BaoCaoMuonSachQuaHan()
        {
            var muonquahan = from m in db.MuonTraSach
                             where m.NgayTra == null && m.HanTra < _end_day_now
                             group m by m.HocSinh into g
                             orderby g.Key.Lop ascending, g.Key.TenHS ascending
                             select new BaoCaoVM { GroupName1 = g.Key.TenHS, GroupSoLuong = g.Count(), GroupDanhSach = g };


            if (muonquahan.Count() > 0)
            {
                ViewBag.MuonQuaHan = muonquahan;
                ViewBag.MuonQuaHan_Count = muonquahan.Sum(m => m.GroupSoLuong);
            }
            else
            {
                ViewBag.MuonQuaHan_Count = 0;
            }
            return View();
        }

        private IQueryable<BaoCaoVM> BaoCaoSachMat()
        {
            var sachmat = from m in db.MuonTraSach
                          where m.Mat == true
                          group m by m.HocSinh into g
                          orderby g.Key.Lop ascending, g.Key.TenHS ascending
                          select new BaoCaoVM { GroupName1 = g.Key.TenHS, GroupSoLuong = g.Count(), GroupDanhSach = g };
            return sachmat;
        }

        private ActionResult BaoCaoThanhLy(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
        {
            if (type.HasValue)
            {
                var danhsachthanhly = from t in db.ThanhLy
                                      select t;

                if (type == LOAIBAOCAO.Thang)
                {
                    if (d.HasValue) // neu du lieu thang co
                    {
                        int _month = d.Value.Month;
                        int _year = d.Value.Year;

                        danhsachthanhly = from dstl in danhsachthanhly
                                          where dstl.Ngay.Month == _month && dstl.Ngay.Year == _year
                                          select dstl;
                    }
                    else // neu ko co du lieu thang
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.Nam)
                {
                    if (d.HasValue) // neu co du lieu nam
                    {
                        int _year = d.Value.Year;
                        danhsachthanhly = from dstl in danhsachthanhly
                                          where dstl.Ngay.Year == _year
                                          select dstl;
                    }
                    else // neu ko co du lieu nam
                    {
                        return View("SthError");
                    }
                }
                else if (type == LOAIBAOCAO.KhoanThoiGian)
                {
                    if ((d.HasValue) && (d_f.HasValue)) // neu co du lieu khoang thoi gian
                    {
                        int _month_s = d.Value.Month;
                        int _year_s = d.Value.Year;
                        int _month_f = d_f.Value.Month;
                        int _year_f = d_f.Value.Year;
                        danhsachthanhly = from dstl in danhsachthanhly
                                          where (dstl.Ngay.Month >= _month_s && dstl.Ngay.Year >= _year_s) && (dstl.Ngay.Month <= _month_f && dstl.Ngay.Year <= _year_f)
                                          select dstl;
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return View("SthError");
                    }
                }
                else
                {
                    return View("SthError");
                }

                if (danhsachthanhly.Count() > 0)
                {
                    ViewBag.ThanhLy_Count = danhsachthanhly.Count();
                    ViewBag.ThanhLy = danhsachthanhly;
                }
                else
                {
                    ViewBag.ThanhLy_Count = 0;
                }
            }
            else
            {
                return View("SthError");
            }

            return View();
        }

        private ActionResult BaoCaoSachTrongThuVien()
        {
            // thong ke sach trong thu vien
            var thongkesach = from s in db.Sach
                              group s by s.ChuDe into g
                              select new BaoCaoVM { GroupName1 = g.Key.TenChuDe, GroupSoLuong = g.Count() };
            if (thongkesach.Count() > 0)
            {
                ViewBag.ThongKeSach = thongkesach;
                ViewBag.ThongKeSach_Count = thongkesach.Sum(t => t.GroupSoLuong);
            }
            else
            {
                ViewBag.ThongKeSach_Count = 0;
            }
            return View();
        }
    }
}
