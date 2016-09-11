using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class ThanhLy
    {
        public int ID { get; set; }
        public int SachID { get; set; }
        [Display(Name = "Ngày Thanh Lý")]
        [DataType(DataType.Date)]
        public DateTime Ngay { get; set; }
        public virtual Sach Sach { get; set; }
    }
}