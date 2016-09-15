using System.Linq;
using System.Web.Mvc;
using LibraryManagementSystem.DAL;
using System;
using LibraryManagementSystem.Models.ViewModel;
using System.Collections.Generic;

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
        public ActionResult LuaChon(LOAIBAOCAO type, DateTime? date, DateTime? date_finish, bool? thanhly_status, bool? muonsach_status)
        {
            return RedirectToAction("Index", "BaoCao", new { type = type, d = date, d_f = date_finish, tl = thanhly_status, ms = muonsach_status });
        }

        // GET: BaoCao
        [Authorize]
        public ActionResult Index(LOAIBAOCAO? type, DateTime? d, DateTime? d_f, bool? tl, bool? ms)
        {
            if (type.HasValue)
            {
                if (d.HasValue || (d.HasValue && d_f.HasValue))
                {
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

        // GET: BaoCao thang
        [Authorize]
        private ActionResult BaoCaoThang(DateTime? d)
        {
            int _month = d.Value.Month;
            int _year = d.Value.Year;

            // muon tra sach trong thang
            var muontrasach = from m in db.MuonTraSach
                              where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year
                              group m by m.HocSinh into g
                              orderby g.Key.Lop ascending, g.Key.TenHS ascending
                              select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };

            if (muontrasach.Count() > 0)
            {
                ViewBag.MuonTraSach_Count = muontrasach.Sum(m => m.GroupSoLuong);
                ViewBag.MuonTraSach = muontrasach;
            }
            else
            {
                ViewBag.MuonTraSach_Count = 0;
            }

            //The loai dc yeu thich
            var theloaiyeuthich = from m in db.MuonTraSach
                                  where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year
                                  group m by m.Sach.ChuDe into g
                                  select new BaoCaoVM { GroupName1 = g.Key.TenChuDe, GroupSoLuong = g.Count() };

            if (theloaiyeuthich.Count() > 0)
            {
                ViewBag.TheLoaiYeuThich = theloaiyeuthich;
            }

            // muon sach chua tra
            var muonchuatra = from m in db.MuonTraSach
                              where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year && m.NgayTra == null
                              group m by m.HocSinh into g
                              orderby g.Key.Lop ascending, g.Key.TenHS ascending
                              select new BaoCaoVM { GroupName1 = g.Key.TenHS, GroupSoLuong = g.Count(), GroupDanhSach = g };

            if (muonchuatra.Count() > 0)
            {
                ViewBag.MuonChuaTra = muonchuatra;
                ViewBag.MuonChuaTra_Count = muonchuatra.Sum(m => m.GroupSoLuong);
            }
            else
            {
                ViewBag.MuonChuaTra_Count = 0;
            }

            // muon sach qua han

            var muonquahan = from m in db.MuonTraSach
                             where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year && m.NgayTra == null && m.HanTra < _end_day_now
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

            // thong ke sach
            var thongkesach = from s in db.Sach
                              group s by s.ChuDe.TenChuDe into g
                              select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count() };
            if (thongkesach.Count() > 0)
            {
                ViewBag.ThongKeSach = thongkesach;
                ViewBag.ThongKeSach_Count = thongkesach.Sum(t => t.GroupSoLuong);
            }
            else
            {
                ViewBag.ThongKeSach_Count = 0;
            }

            ViewBag.date = _month + "/" + _year;
            return View("~/Views/BaoCao/Index.cshtml");

        }

        // GET: BaoCao Nam
        [Authorize]
        private ActionResult BaoCaoNam(DateTime? d)
        {
            int _year = d.Value.Year;

            // muon tra sach trong nam 
            var muontrasach = from m in db.MuonTraSach
                              where m.NgayMuon.Year == _year
                              group m by m.HocSinh into g
                              orderby g.Key.Lop ascending, g.Key.TenHS ascending
                              select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };

            if (muontrasach.Count() > 0)
            {
                ViewBag.MuonTraSach_Count = muontrasach.Sum(m => m.GroupSoLuong);
                ViewBag.MuonTraSach = muontrasach;
            }
            else
            {
                ViewBag.MuonTraSach_Count = 0;
            }

            //The loai dc yeu thich
            var theloaiyeuthich = from m in db.MuonTraSach
                                  where m.NgayMuon.Year == _year
                                  group m by m.Sach.ChuDe into g
                                  select new BaoCaoVM { GroupName1 = g.Key.TenChuDe, GroupSoLuong = g.Count() };

            if (theloaiyeuthich.Count() > 0)
            {
                ViewBag.TheLoaiYeuThich = theloaiyeuthich;
            }

            // muon sach chua tra
            var muonchuatra = from m in db.MuonTraSach
                              where m.NgayMuon.Year == _year && m.NgayTra == null
                              group m by m.HocSinh into g
                              orderby g.Key.Lop ascending, g.Key.TenHS ascending
                              select new BaoCaoVM { GroupName1 = g.Key.TenHS, GroupSoLuong = g.Count(), GroupDanhSach = g };

            if (muonchuatra.Count() > 0)
            {
                ViewBag.MuonChuaTra = muonchuatra;
                ViewBag.MuonChuaTra_Count = muonchuatra.Sum(m => m.GroupSoLuong);
            }
            else
            {
                ViewBag.MuonChuaTra_Count = 0;
            }

            // muon sach qua han

            var muonquahan = from m in db.MuonTraSach
                             where m.NgayMuon.Year == _year && m.NgayTra == null && m.HanTra < _end_day_now
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

            // thong ke sach
            var thongkesach = from s in db.Sach
                              group s by s.ChuDe.TenChuDe into g
                              select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count() };
            if (thongkesach.Count() > 0)
            {
                ViewBag.ThongKeSach = thongkesach;
                ViewBag.ThongKeSach_Count = thongkesach.Sum(t => t.GroupSoLuong);
            }
            else
            {
                ViewBag.ThongKeSach_Count = 0;
            }

            ViewBag.date = _year.ToString();
            return View("~/Views/BaoCao/Index.cshtml");

        }

        // GET: BaoCao Nam
        [Authorize]
        private ActionResult BaoCaoKTG(DateTime? d_s, DateTime? d_f)
        {
            int _month_s = d_s.Value.Month;
            int _year_s = d_s.Value.Year;
            int _month_f = d_f.Value.Month;
            int _year_f = d_f.Value.Year;
            // muon tra sach trong nam 
            var muontrasach = from m in db.MuonTraSach
                              where (m.NgayMuon.Month >= _month_s && m.NgayMuon.Year >= _year_s) && (m.NgayMuon.Month <= _month_f && m.NgayMuon.Year <= _year_f)
                              group m by m.HocSinh into g
                              orderby g.Key.Lop ascending, g.Key.TenHS ascending
                              select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupDatetime = g.Key.NgaySinh, GroupSoLuong = g.Count() };

            if (muontrasach.Count() > 0)
            {
                ViewBag.MuonTraSach_Count = muontrasach.Sum(m => m.GroupSoLuong);
                ViewBag.MuonTraSach = muontrasach;
            }
            else
            {
                ViewBag.MuonTraSach_Count = 0;
            }

            //The loai dc yeu thich
            var theloaiyeuthich = from m in db.MuonTraSach
                                  where (m.NgayMuon.Month >= _month_s && m.NgayMuon.Year >= _year_s) && (m.NgayMuon.Month <= _month_f && m.NgayMuon.Year <= _year_f)
                                  group m by m.Sach.ChuDe into g
                                  select new BaoCaoVM { GroupName1 = g.Key.TenChuDe, GroupSoLuong = g.Count() };

            if (theloaiyeuthich.Count() > 0)
            {
                ViewBag.TheLoaiYeuThich = theloaiyeuthich;
            }

            // muon sach chua tra
            var muonchuatra = from m in db.MuonTraSach
                              where (m.NgayMuon.Month >= _month_s && m.NgayMuon.Year >= _year_s) && (m.NgayMuon.Month <= _month_f && m.NgayMuon.Year <= _year_f)
                              group m by m.HocSinh into g
                              orderby g.Key.Lop ascending, g.Key.TenHS ascending
                              select new BaoCaoVM { GroupName1 = g.Key.TenHS, GroupSoLuong = g.Count(), GroupDanhSach = g };

            if (muonchuatra.Count() > 0)
            {
                ViewBag.MuonChuaTra = muonchuatra;
                ViewBag.MuonChuaTra_Count = muonchuatra.Sum(m => m.GroupSoLuong);
            }
            else
            {
                ViewBag.MuonChuaTra_Count = 0;
            }

            // muon sach qua han

            var muonquahan = from m in db.MuonTraSach
                             where (m.NgayMuon.Month >= _month_s && m.NgayMuon.Year >= _year_s) && (m.NgayMuon.Month <= _month_f && m.NgayMuon.Year <= _year_f) && (m.NgayTra > m.HanTra)
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

            // thong ke sach
            var thongkesach = from s in db.Sach
                              group s by s.ChuDe.TenChuDe into g
                              select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count() };
            if (thongkesach.Count() > 0)
            {
                ViewBag.ThongKeSach = thongkesach;
                ViewBag.ThongKeSach_Count = thongkesach.Sum(t => t.GroupSoLuong);
            }
            else
            {
                ViewBag.ThongKeSach_Count = 0;
            }

            ViewBag.date = _month_s + "/" + _year_s + " - " + _month_f + "/" + _year_f;
            return View("~/Views/BaoCao/Index.cshtml");

        }
    }
}
