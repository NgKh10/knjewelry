// Models/Entities/Wishlist.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("YeuThich")]
    public class YeuThich
    {
        [Key]
        public int id { get; set; }

        [Required]
        public int id_nguoi_dung { get; set; }

        [Required]
        public int id_san_pham { get; set; }

        public DateTime ngay_tao { get; set; } = DateTime.Now;

        [ForeignKey("id_nguoi_dung")]
        public virtual NguoiDung NguoiDung { get; set; }

        [ForeignKey("id_san_pham")]
        public virtual SanPham SanPham { get; set; }
    }
}