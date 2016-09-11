using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models.ViewModel
{
    public class BaoCaoVM
    {
        public string GroupName1 { get; set;}
        public string GroupName2 { get; set; }
        public string GroupName3 { get; set; }
        public int GroupSoLuong { get; set; }
        public IEnumerable<MuonTraSach> GroupDanhSach { get; set; }
        
    }
}