using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class MuonTraSach
    {
        public int ID { get; set; }
        public int SachID { get; set; }
        public int HocSinhID { get; set; }

        [Display(Name = "Ngày Mượn"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
        [Required(ErrorMessage = "Bạn cần nhập Ngày Mượn")]
        public DateTime NgayMuon { get; set; }

        [Display(Name = "Hạn Trả Sách"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
        public DateTime HanTra { get; set; }

        [Display(Name = "Ngày Trả Sách"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", NullDisplayText = "Chưa trả")]
        public DateTime? NgayTra { get; set; }

        [Display(Name ="Bị Mất")]
        public bool Mat { get; set; }

        public virtual HocSinh HocSinh { get; set; }
        public virtual Sach Sach { get; set; }
    }
}