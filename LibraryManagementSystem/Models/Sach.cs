using LibraryManagementSystem.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Sach
    {
        public int ID { get; set; }
        public int ChuDeID { get; set; }
        [Display(Name = "Mã Số Sách"), Required(ErrorMessage = "Bạn cần nhập Mã Số Sách")]
        public string SachID { get; set; }
        [Display(Name = "Tên Sách"), Required(ErrorMessage = "Bạn cần nhập Tên Sách")]
        public string TenSach { get; set; }
        [Display(Name = "Trạng Thái")]
        public TrangThai TrangThai { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày Nhập"), Required(ErrorMessage = "Bạn cần nhập ngày nhập sách")]
        public DateTime NgayNhap { get; set; }
        public string IDandTen
        {
            get { return SachID + " - " + TenSach; }
        }
        public virtual ChuDe ChuDe { get; set; }
        public virtual ICollection<MuonTraSach> MuonTraSach { get; set; }
        public virtual ICollection<ThanhLy> ThanhLy { get; set; }
    }
}