using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public enum TrangThai
    {
        [Display(Name = "Có Sẵn",Description = "Có Sẵn")]
        CoSan,

        [Display(Name = "Đang Được Mượn", Description = "Đang Được Mượn")]
        DangMuon,

        [Display(Name = "Mất", Description = "Mất")]
        Mat,
        [Display(Name = "Đã Thanh Lý", Description = "Đã Thanh Lý")]
        ThanhLy
    }
    public class Sach
    {
        
        public int ID { get; set; }
        public int ChuDeID { get; set; }

        [Display(Name = "Mã Số Sách"), Required(ErrorMessage = "Bạn cần nhập Mã Số Sách")]
        public string SachID { get; set; }

        [Display(Name = "Tên Sách"), Required(ErrorMessage = "Bạn cần nhập Tên Sách")]
        public string TenSach { get; set; }

        [Display(Name = "Số Lượng"), Required(ErrorMessage = "Bạn cần nhập Số Lượng")]
        [Range(minimum:0, maximum: int.MaxValue, ErrorMessage = "Số lượng phải lớn 0")]
        public int SoLuong { get; set; }

        [Display(Name = "Trạng Thái")]
        public TrangThai TrangThai { get; set; }

        public string IDandTen
        {
            get
            {
                return SachID + " - " + TenSach;
            }
        }

        public virtual ChuDe ChuDe { get; set; }
        public virtual ICollection<MuonTraSach> MuonTraSach { get; set; }
        public virtual ICollection<ThanhLy> ThanhLy { get; set; }
    }
}