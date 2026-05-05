using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("ChiTietGioHang")]
    public class ChiTietGioHang
    {
        [Key]
        public int id_chi_tiet_gh { get; set; }

        [Required]
        public int id_gio_hang { get; set; }

        [Required]
        public int id_san_pham { get; set; }

        public int? id_bien_the { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int so_luong { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal don_gia { get; set; }

        public DateTime ngay_tao { get; set; } = DateTime.Now;

        [ForeignKey("id_gio_hang")]
        public virtual GioHang GioHang { get; set; }

        [ForeignKey("id_san_pham")]
        public virtual SanPham SanPham { get; set; }

        [ForeignKey("id_bien_the")]
        public virtual BienThe BienThe { get; set; }
    }
}