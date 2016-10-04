using System.Linq;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using System;
using LibraryManagementSystem.Models.ViewModel;
using LibraryManagementSystem.Models;
using System.IO;
using Novacode;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

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

        public static LOAIBAOCAO thisistype;
        public static DateTime thisisD;
        public static DateTime thisisD_F;
        private CNCFContext db = new CNCFContext();

        private DateTime _end_day_now = CNCFClass.GoToEndOfDay(DateTime.Now);

        [HttpPost]
        [Authorize]
        public ActionResult LuaChon(LOAIBAOCAO type, DateTime? date, DateTime? date_finish, bool? thanhly_status, bool? muonsach_status, bool? docsach_status, bool? theloaiyeuthich_status, bool? sachyeuthich_status, bool? sachmat_status, bool? muonsachquahan_status, bool? sachthuvien_status)
        {
            thisistype = type;
            thisisD = date.Value;
            if (date_finish != null)
            {
                thisisD_F = date_finish.Value;
            }

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

                    #region Set Du Lieu Thoi Gian
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
                        ViewBag.date = d.Value.Month + "/" + d.Value.Year + " ‒ " + d_f.Value.Month + "/" + d_f.Value.Year;
                    }
                    #endregion
                    #region Muon Sach Ve Nha
                    // Báo Cáo Mượn sách
                    if (ms.HasValue)
                    {
                        ViewBag.muonsach_stt = ms;
                        if (ms.Value)
                        {
                            var danhsachmuonsach = BaoCaoMuonSach(type, d, d_f);

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
                    }
                    else
                    {
                        ViewBag.muonsach_stt = false;
                    }
                    #endregion
                    #region Bao Cao Doc Sach
                    // Báo Cáo Đọc sách
                    if (ds.HasValue)
                    {
                        ViewBag.docsach_stt = ds;
                        if (ds.Value)
                        {
                            var danhsachdoc = BaoCaoDocSach(type, d, d_f);
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
                    }
                    else
                    {
                        ViewBag.docsach_stt = false;
                    }
                    #endregion
                    #region The Loai Sach Yeu Thich
                    // Báo Cáo Thể Loại Sách Yêu Thích
                    if (tlyt.HasValue)
                    {
                        ViewBag.theloaiyeuthich_stt = tlyt;
                        if (tlyt.Value)
                        {
                            var theloaiyeuthich = BaoCaoTheLoaiYeuThich(type, d, d_f);
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
                    }
                    else
                    {
                        ViewBag.theloaiyeuthich_stt = false;
                    }
                    #endregion
                    #region Sach Yeu Thich
                    // Báo Cáo SÁch yêu thích
                    if (syt.HasValue)
                    {
                        ViewBag.sachyeuthich_stt = syt;
                        if (syt.Value)
                        {
                            var sachyeuthich = BaoCaoSachYeuThich(type, d, d_f);
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
                    }
                    else
                    {
                        ViewBag.sachyeuthich_stt = false;
                    }
                    #endregion
                    #region Muon Sach Qua Han
                    // Báo Cáo Mượn sách quá hạn
                    if (msqh.HasValue)
                    {
                        ViewBag.muonsachquahan_stt = msqh;
                        if (msqh.Value)
                        {
                            var muonquahan = BaoCaoMuonSachQuaHan();
                            if (muonquahan.Count() > 0)
                            {
                                ViewBag.MuonQuaHan = muonquahan;
                                ViewBag.MuonQuaHan_Count = muonquahan.Sum(m => m.GroupSoLuong);
                            }
                            else
                            {
                                ViewBag.MuonQuaHan_Count = 0;
                            }
                        }
                    }
                    else
                    {
                        ViewBag.muonsachquahan_stt = false;
                    }
                    #endregion
                    #region Tong Sach Trong Thu Vien
                    // Báo Cáo Số lượng sách trong thư viện theo the loai
                    if (stv.HasValue)
                    {
                        ViewBag.sachtrongthuvien_stt = stv;
                        if (stv.Value)
                        {
                            var thongkesach = BaoCaoSachTrongThuVien();
                            if (thongkesach.Count() > 0)
                            {
                                ViewBag.ThongKeSach = thongkesach;
                                ViewBag.ThongKeSach_Count = thongkesach.Sum(t => t.GroupSoLuong);
                            }
                            else
                            {
                                ViewBag.ThongKeSach_Count = 0;
                            }
                        }
                    }
                    else
                    {
                        ViewBag.sachtrongthuvien_stt = false;
                    }
                    #endregion
                    #region Thanh Ly
                    // Báo cáo Thanh lý
                    if (tl.HasValue)
                    {
                        ViewBag.thanhly_stt = tl;
                        if (tl.Value)
                        {
                            var danhsachthanhly = BaoCaoThanhLy(type, d, d_f);
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
                    }
                    else
                    {
                        ViewBag.thanhly_stt = false;
                    }
                    #endregion
                    #region Sach Mat
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
                    #endregion

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

        private IQueryable<BaoCaoVM> BaoCaoDocSach(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
        {
            thisistype = type.Value;
            thisisD = d.Value;
            if (d_f != null)
            {
                thisisD_F = d_f.Value;
            }
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
                                          group dsd by dsd.HocSinh.Lop into g
                                          orderby g.Key ascending
                                          select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count() };

                        return danhsachdoc;
                    }
                    else // neu ko co du lieu thang
                    {
                        return null;
                    }
                }
                else if (type == LOAIBAOCAO.Nam)
                {
                    if (d.HasValue) // neu co du lieu nam
                    {
                        int _year = d.Value.Year;

                        var danhsachdoc = from dsd in db.DocSachTaiCho
                                          where dsd.Ngay.Year == _year
                                          group dsd by dsd.HocSinh.Lop into g
                                          orderby g.Key ascending
                                          select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count() };
                        return danhsachdoc;
                    }
                    else // neu ko co du lieu nam
                    {
                        return null;
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
                                          group dsd by dsd.HocSinh.Lop into g
                                          orderby g.Key ascending
                                          select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count() };
                        return danhsachdoc;
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }

        private IQueryable<BaoCaoVM> BaoCaoMuonSach(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
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
                        return danhsachmuonsach;
                    }
                    else // neu ko co du lieu thang
                    {
                        return null;
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
                        return danhsachmuonsach;
                    }
                    else // neu ko co du lieu nam
                    {
                        return null;
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
                        return danhsachmuonsach;
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
        }

        private IQueryable<BaoCaoVM> BaoCaoTheLoaiYeuThich(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
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

                        return theloaiyeuthich;
                    }
                    else // neu ko co du lieu thang
                    {
                        return null;
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

                        return theloaiyeuthich;
                    }
                    else // neu ko co du lieu nam
                    {
                        return null;
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
                        return theloaiyeuthich;
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return null;
                    }
                }
                else // type ko xác định
                {
                    return null;
                }

            }
            else // type ko có dữ liệu
            {
                return null;
            }
        }

        private IQueryable<BaoCaoVM> BaoCaoSachYeuThich(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
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
                        return sachyeuthich;
                    }
                    else // neu ko co du lieu thang
                    {
                        return null;
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
                        return sachyeuthich;

                    }
                    else // neu ko co du lieu nam
                    {
                        return null;
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

                        return sachyeuthich;
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return null;
                    }
                }
                else // type ko xác định
                {
                    return null;
                }

            }
            else // type ko có dữ liệu
            {
                return null;
            }
        }

        private IQueryable<BaoCaoVM> BaoCaoMuonSachQuaHan()
        {
            var muonquahan = from m in db.MuonTraSach
                             where m.NgayTra == null && m.HanTra < _end_day_now
                             group m by m.HocSinh into g
                             orderby g.Key.Lop ascending, g.Key.TenHS ascending
                             select new BaoCaoVM { GroupName1 = g.Key.TenHS, GroupName2 = g.Key.Lop, GroupSoLuong = g.Count(), GroupDanhSach = g };

            return muonquahan;
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

        private IQueryable<ThanhLy> BaoCaoThanhLy(LOAIBAOCAO? type, DateTime? d, DateTime? d_f)
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
                        return danhsachthanhly;
                    }
                    else // neu ko co du lieu thang
                    {
                        return null;
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
                        return danhsachthanhly;
                    }
                    else // neu ko co du lieu nam
                    {
                        return null;
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
                        return danhsachthanhly;
                    }
                    else // neu ko co du lieu khoang thoi gian
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private IQueryable<BaoCaoVM> BaoCaoSachTrongThuVien()
        {
            // thong ke sach trong thu vien
            var thongkesach = from s in db.Sach
                              group s by s.ChuDe into g
                              select new BaoCaoVM { GroupName1 = g.Key.TenChuDe, GroupSoLuong = g.Count() };
            return thongkesach;
        }

        // Xuat Bao Cao ra file word
        [HttpPost]
        [Authorize]
        public ActionResult XuatBaoCao(bool? docsach, bool? muonsach, bool? theloaiyt, bool? sachyt, bool? quahan, bool? thanhly, bool? sachmat, bool? sachtv, string date)
        {
            Dictionary<string, int> aoe = new Dictionary<string, int>();
            // Khoi tao bo nho
            MemoryStream stream = new MemoryStream();
            // Khoi tao file Word
            DocX doc = DocX.Create(stream);
            // Tao Font su dung
            FontFamily TNR = new FontFamily("Times New Roman");
            doc.InsertParagraph("Thư viện Trường TH Ánh Sáng").FontSize(13).Font(new FontFamily("Times New Roman")).Bold();
            // Hang tieu de
            Paragraph p_Title = doc.InsertParagraph();
            p_Title.Append("BÁO CÁO HOẠT ĐỘNG THƯ VIỆN\n").FontSize(13).Font(new FontFamily("Times New Roman")).Bold();
            if (date != null)
            {
                string KhoanThoiGian = string.Format("(Từ tháng {0})\n", date);
                p_Title.Append(KhoanThoiGian).FontSize(13).Font(new FontFamily("Times New Roman"));
            }
            // Dinh dang tieu de
            p_Title.Alignment = Alignment.center;

            Paragraph heading = doc.InsertParagraph();
            heading.StyleName = "Heading1";

            #region Bao Cao Muon Sach + Doc Tai Cho
            ///
            ///Bao cao muon sach
            ///
            // Check neu muon tao bao cao Muon sach
            var bcMuonsach = BaoCaoMuonSach(thisistype, thisisD, thisisD_F);
            int value = 0;
            Dictionary<string, int> bc_muonsachFilter = new Dictionary<string, int>();
            heading.AppendLine("1. Hoạt động đọc sách:");
            heading.FontSize(13).Font(TNR);
            //Loc ra theo lop
            foreach (var v in bcMuonsach)
            {
                if (bc_muonsachFilter.TryGetValue(v.GroupName1, out value))
                {
                    bc_muonsachFilter[v.GroupName1] += v.GroupSoLuong;
                }
                else
                {
                    bc_muonsachFilter.Add(v.GroupName1, v.GroupSoLuong);
                }
            }
            Paragraph p_bcDocsach = doc.InsertParagraph();

            p_bcDocsach.AppendLine("Bảng kê số lượt mượn sách về nhà:").FontSize(13).Font(TNR).Bold();
            int CellIndex = bc_muonsachFilter.Count();

            ///Check so sanh voi bcDocSach


            //if (docsach != null)
            //{
            CellIndex += 2;
            //}
            //else
            //{
            //    CellIndex += 1;
            //}

            Table t_Muonsach = doc.AddTable(2, CellIndex);
            t_Muonsach.Design = TableDesign.TableGrid;
            int Row = 0;
            int Cell = 0;
            {
                int tc_luot = 0;
                // Xuat so luong tung lop
                foreach (KeyValuePair<string, int> lop in bc_muonsachFilter)
                {

                    t_Muonsach.Rows[Row].Cells[Cell].Paragraphs[0].Append(string.Format("{0}", lop.Key)).FontSize(13).Font(TNR);
                        t_Muonsach.Rows[Row + 1].Cells[Cell].Paragraphs[0].Append(string.Format("{0}", lop.Value)).FontSize(13).Font(TNR);
                    tc_luot += lop.Value;
                        Cell++;
                }
                //Xuat tong so luon cac lop
                t_Muonsach.Rows[Row].Cells[Cell].Paragraphs[0].Append(string.Format("Tổng số lượt mượn sách")).FontSize(13).Font(TNR);
                t_Muonsach.Rows[Row + 1].Cells[Cell].Paragraphs[0].Append(string.Format("{0}", tc_luot)).FontSize(13).Font(TNR);
                Cell++;
                //So sanh voi doc sach tai cho (Neu co)
                //if (docsach != null)
                //{
                var bcDocsach = BaoCaoDocSach(thisistype, thisisD, thisisD_F);
                int tc_luotDoc = 0;
                foreach (var v in bcDocsach)
                {
                    tc_luotDoc += v.GroupSoLuong;
                }
                t_Muonsach.Rows[Row].Cells[Cell].Paragraphs[0].Append(string.Format("Tổng số lượt đọc sách tại chỗ")).FontSize(13).Font(TNR);
                t_Muonsach.Rows[Row + 1].Cells[Cell].Paragraphs[0].Append(string.Format("{0}", tc_luotDoc)).FontSize(13).Font(TNR);
                //}
            }
            t_Muonsach.Alignment = Alignment.center;
            //In ra file
            p_bcDocsach.InsertTableAfterSelf(t_Muonsach);
            p_bcDocsach.AppendLine("");
            #region Bao Cao Sach Qua Han
            // Sach muon chua tra
            Paragraph heading2 = doc.InsertParagraph();
            heading2.StyleName = "Heading1";
            heading2.AppendLine("2. Sách mượn quá hạn:").FontSize(13).Font(TNR);

            var bc_Quahang = BaoCaoMuonSachQuaHan();
            Dictionary<string, int> filter_quahang = new Dictionary<string, int>();

            foreach (var v in bc_Quahang)
            {
                if (filter_quahang.TryGetValue(v.GroupName2, out value))
                {
                    filter_quahang[v.GroupName2] += v.GroupSoLuong;
                }
                else
                {
                    filter_quahang.Add(v.GroupName2, v.GroupSoLuong);
                }
                
            }
            // Filter sach qua han



            Paragraph bc_Trasach = doc.InsertParagraph();
            bc_Trasach.AppendLine("Trường Ánh Sáng").FontSize(13).Font(TNR).Italic();
            foreach (KeyValuePair<string, int> lop in filter_quahang)
            {
                bc_Trasach.AppendLine(string.Format("{0}:", lop.Key)).FontSize(14).Font(TNR).Bold();
                foreach (var v in bc_Quahang)
                {
                    if (lop.Key == v.GroupName2)
                    {
                        StringBuilder moihs = new StringBuilder();
                        moihs.AppendFormat(" - {0}:", v.GroupName1);
                        foreach (var vs in v.GroupDanhSach)
                        {
                            moihs.AppendFormat(" {0},", vs.Sach.TenSach);
                        }
                        moihs[moihs.Length - 1] = '.';
                        bc_Trasach.AppendLine(moihs.ToString()).FontSize(13).Font(TNR);
                    }
                }
            }
            bc_Trasach.AppendLine("");

            #endregion

            // Sach bi mat
            bc_Trasach.AppendLine("Sách học sinh làm mất:").FontSize(14).Font(TNR).Bold().UnderlineColor(Color.Black);
            var bcMatsach = BaoCaoSachMat();
            foreach (var v in bcMatsach)
            {
                string sen = string.Format("{0}:", v.GroupName1);
                foreach (var vs in v.GroupDanhSach)
                {
                    sen += string.Format(" {0},", vs.Sach.TenSach);
                }
                StringBuilder text = new StringBuilder(sen);
                text[text.Length - 1] = '.';
                sen = text.ToString();
                bc_Trasach.AppendLine(sen).FontSize(13).Font(TNR);
            }
            #endregion
            #region Sach cua thu vien
            Paragraph heading3 = doc.InsertParagraph();
            heading3.StyleName = "Heading1";
            heading3.AppendLine("3. Sách của thư viện").FontSize(13).Font(TNR);

            Paragraph p_bcSach = doc.InsertParagraph();
            p_bcSach.AppendLine("Bảng thống kê sách:").FontSize(13).Font(TNR).Bold();

            Paragraph t_Sach = doc.InsertParagraph();
            var bcSach = BaoCaoSachTrongThuVien();
            int soCD = bcSach.Count();
            Table t_bcSach = doc.AddTable(soCD + 2, 2);
            t_bcSach.Design = TableDesign.TableGrid;
            int tCell = 0;
            int tRow = 0;
            int tc_sach = 0;
            t_bcSach.Rows[tRow].Cells[tCell].Paragraphs[0].Append("THỂ LOẠI SÁCH").FontSize(13).Font(TNR);
            t_bcSach.Rows[tRow].Cells[tCell + 1].Paragraphs[0].Append("SỐ LƯỢNG").FontSize(13).Font(TNR);
            tRow++;
            foreach (var v in bcSach)
            {
                t_bcSach.Rows[tRow].Cells[tCell].Paragraphs[0].Append(string.Format("{0}", v.GroupName1)).FontSize(13).Font(TNR);
                t_bcSach.Rows[tRow].Cells[tCell + 1].Paragraphs[0].Append(string.Format("{0}", v.GroupSoLuong)).FontSize(13).Font(TNR);
                tc_sach += v.GroupSoLuong;
                tRow++;
            }
            t_bcSach.Rows[tRow].Cells[tCell].Paragraphs[0].Append(string.Format("Tổng cộng")).FontSize(13).Font(TNR).Italic();
            t_bcSach.Rows[tRow].Cells[tCell + 1].Paragraphs[0].Append(string.Format("{0}", tc_sach)).FontSize(13).Font(TNR);
            t_bcSach.Alignment = Alignment.left;
            t_Sach.InsertTableAfterSelf(t_bcSach);
            #endregion
            doc.Save();
            string filename = "BaoCao_" + date.Replace('/', '.') + ".doc";
            return File(stream.ToArray(), "application/octet-stream", filename);
        }
    }
}
