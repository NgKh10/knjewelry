using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("GioHang")]
    public class GioHang
    {
        [Key]
        public int id_gio_hang { get; set; }

        [Required]
        [MaxLength(100)]
        public string ma_phien { get; set; }

        public int? id_nguoi_dung { get; set; }

        public DateTime ngay_tao { get; set; } = DateTime.Now;

        public DateTime ngay_cap_nhat { get; set; } = DateTime.Now;

        [ForeignKey("id_nguoi_dung")]
        public virtual NguoiDung NguoiDung { get; set; }

        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; }
    }
}