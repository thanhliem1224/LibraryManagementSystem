using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class HocSinh
    {
        public int ID { get; set; }
        [Display(Name = "Tên học sinh"), Required(ErrorMessage = "Bạn cần nhập Tên Học Sinh")]
        public string TenHS { get; set; }

        [Display(Name = "Lớp"), Required(ErrorMessage = "Bạn cần nhập Lớp")]
        public string Lop { get; set; }

        [Display(Name = "Ngày sinh"), Required(ErrorMessage = "Bạn cần nhập Ngày Sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgaySinh { get; set; }

        public virtual ICollection<MuonTraSach> MuonTraSach { get; set; }
    }
}