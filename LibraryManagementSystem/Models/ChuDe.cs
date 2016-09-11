using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class ChuDe
    {
        public int ID { get; set; }

        [Display(Name = "Tên Chủ Đề")]
        [Required(ErrorMessage = "Bạn cần nhập Ten Chủ Đề")]
        public string TenChuDe { get; set; }
        public virtual ICollection<Sach> Sach { get; set; }
    }
}