using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class MuonTraSach
    {
        public int ID { get; set; }
        public int SachID { get; set; }
        public int HocSinhID { get; set; }

        [Display(Name = "Ngày Mượn"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [Required(ErrorMessage = "Bạn cần nhập Ngày Mượn")]
        public DateTime NgayMuon { get; set; }

        [Display(Name = "Hạn Trả Sách"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime HanTra { get; set; }

        [Display(Name = "Ngày Trả Sách"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", NullDisplayText = "Chưa trả")]
        public DateTime? NgayTra { get; set; }

        public virtual HocSinh HocSinh { get; set; }
        public virtual Sach Sach { get; set; }
    }
}