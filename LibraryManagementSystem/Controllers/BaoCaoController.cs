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
        public ActionResult LuaChon(LOAIBAOCAO type, DateTime? date_month, DateTime? date_year, DateTime? date_start, DateTime? date_finish)
        {
            return RedirectToAction("Index", "BaoCao", new { type = type, d_m = date_month, d_y = date_year, d_s = date_start, d_f = date_finish });
        }

        // GET: BaoCao
        [Authorize]
        public ActionResult Index(LOAIBAOCAO? type, DateTime? d_m, DateTime? d_y, DateTime? d_s, DateTime? d_f)
        {
            if (type.HasValue)
            {
                switch (type)
                {
                    case LOAIBAOCAO.Thang:
                        {
                            if (d_m.HasValue)
                            {
                                BaoCaoThang(d_m); return View();
                            }
                            else
                            {
                                return View("SthError");
                            }
                        }
                    case LOAIBAOCAO.Nam:
                        {
                            if (d_y.HasValue)
                            {
                                BaoCaoNam(d_y); return View();
                            }
                            else
                            {
                                return View("SthError");
                            }
                        }
                    case LOAIBAOCAO.KhoanThoiGian:
                        {
                            if (d_s.HasValue && d_f.HasValue)
                            {
                                BaoCaoKTG(d_s,d_f); return View();
                            }
                            else
                            {
                                return View("SthError");
                            }
                        }
                    default: return View("SthError");
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

        // GET: BaoCao thang
        [Authorize]
        private ActionResult BaoCaoThang(DateTime? d)
        {
            int _month = d.Value.Month;
            int _year = d.Value.Year;

            // muon tra sach trong thang 
            var muontrasach = from m in db.MuonTraSach
                              where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year
                              group m by new { m.HocSinh.Lop, m.HocSinh.TenHS } into g
                              select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupSoLuong = g.Count() };

            if (muontrasach.Count() > 0)
            {
                ViewBag.MuonTraSach_Count = muontrasach.Sum(m => m.GroupSoLuong);
                ViewBag.MuonTraSach = muontrasach;
            }
            else
            {
                ViewBag.MuonTraSach_Count = 0;
            }

            // muon sach chua tra
            var muonchuatra = from m in db.MuonTraSach
                              where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year && m.NgayTra == null
                              group m by m.HocSinh.TenHS into g
                              select new BaoCaoSachChuaTra { TenHS = g.Key, SoLuong = g.Count(), DanhSachMuon = g };

            if (muonchuatra.Count() > 0)
            {
                ViewBag.MuonChuaTra = muonchuatra;
                ViewBag.MuonChuaTra_Count = muonchuatra.Sum(m => m.SoLuong);
            }
            else
            {
                ViewBag.MuonChuaTra_Count = 0;
            }

            // muon sach qua han
            var muonquahan = from m in db.MuonTraSach
                             where m.NgayMuon.Month == _month && m.NgayMuon.Year == _year && m.NgayTra == null && m.HanTra < _end_day_now
                             group m by m.HocSinh.TenHS into g
                             select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count(), GroupDanhSach = g };

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

            // muon tra sach trong thang 
            var muontrasach = from m in db.MuonTraSach
                              where m.NgayMuon.Year == _year
                              group m by new { m.HocSinh.Lop, m.HocSinh.TenHS } into g
                              select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupSoLuong = g.Count() };

            if (muontrasach.Count() > 0)
            {
                ViewBag.MuonTraSach_Count = muontrasach.Sum(m => m.GroupSoLuong);
                ViewBag.MuonTraSach = muontrasach;
            }
            else
            {
                ViewBag.MuonTraSach_Count = 0;
            }

            // muon sach chua tra
            var muonchuatra = from m in db.MuonTraSach
                              where m.NgayMuon.Year == _year && m.NgayTra == null
                              group m by m.HocSinh.TenHS into g
                              select new BaoCaoSachChuaTra { TenHS = g.Key, SoLuong = g.Count(), DanhSachMuon = g };

            if (muonchuatra.Count() > 0)
            {
                ViewBag.MuonChuaTra = muonchuatra;
                ViewBag.MuonChuaTra_Count = muonchuatra.Sum(m => m.SoLuong);
            }
            else
            {
                ViewBag.MuonChuaTra_Count = 0;
            }

            // muon sach qua han

            var muonquahan = from m in db.MuonTraSach
                             where m.NgayMuon.Year == _year && m.NgayTra == null && m.HanTra < _end_day_now
                             group m by m.HocSinh.TenHS into g
                             select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count(), GroupDanhSach = g };

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
            // muon tra sach trong khoang thoi gian
            var muontrasach = from m in db.MuonTraSach
                              where (m.NgayMuon.Month >= _month_s && m.NgayMuon.Year >= _year_s) && (m.NgayMuon.Month <= _month_f && m.NgayMuon.Year <= _year_f)
                              group m by new { m.HocSinh.Lop, m.HocSinh.TenHS } into g
                              select new BaoCaoVM { GroupName1 = g.Key.Lop, GroupName2 = g.Key.TenHS, GroupSoLuong = g.Count() };

            if (muontrasach.Count() > 0)
            {
                ViewBag.MuonTraSach_Count = muontrasach.Sum(m => m.GroupSoLuong);
                ViewBag.MuonTraSach = muontrasach;
            }
            else
            {
                ViewBag.MuonTraSach_Count = 0;
            }

            // muon sach chua tra
            var muonchuatra = from m in db.MuonTraSach
                              where (m.NgayMuon.Month >= _month_s && m.NgayMuon.Year >= _year_s) && (m.NgayMuon.Month <= _month_f && m.NgayMuon.Year <= _year_f) && (m.NgayTra == null)
                              group m by m.HocSinh.TenHS into g
                              select new BaoCaoSachChuaTra { TenHS = g.Key, SoLuong = g.Count(), DanhSachMuon = g };

            if (muonchuatra.Count() > 0)
            {
                ViewBag.MuonChuaTra = muonchuatra;
                ViewBag.MuonChuaTra_Count = muonchuatra.Sum(m => m.SoLuong);
            }
            else
            {
                ViewBag.MuonChuaTra_Count = 0;
            }

            // muon sach qua han

            var muonquahan = from m in db.MuonTraSach
                             where (m.NgayMuon.Month >= _month_s && m.NgayMuon.Year >= _year_s) && (m.NgayMuon.Month <= _month_f && m.NgayMuon.Year <= _year_f) && (m.NgayTra > m.HanTra)
                             group m by m.HocSinh.TenHS into g
                             select new BaoCaoVM { GroupName1 = g.Key, GroupSoLuong = g.Count(), GroupDanhSach = g };

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
