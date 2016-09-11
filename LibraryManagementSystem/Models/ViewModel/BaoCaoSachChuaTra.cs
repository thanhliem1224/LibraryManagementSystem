using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models.ViewModel
{
    public class BaoCaoSachChuaTra
    {
        public string TenHS { get; set; }
        public int SoLuong { get; set; }
        public IEnumerable<MuonTraSach> DanhSachMuon { get; set; }
    }
}