using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models
{
    public class DocSachTaiCho
    {
        public int ID { get; set; }
        public int HocSinhID { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Ngày")]
        public DateTime Ngay { get; set; }
        public virtual HocSinh HocSinh { get; set; }

    }
}