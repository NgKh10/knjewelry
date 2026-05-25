using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("ChiTietGioHang")]
    public class ChiTietGioHang
    {
        [Key]
        public int id_chi_tiet_gh { get; set; }

        [ForeignKey("GioHang")]
        public int id_gio_hang { get; set; }

        [ForeignKey("SanPham")]
        public int id_san_pham { get; set; }

        [ForeignKey("BienThe")]
        public int? id_bien_the { get; set; }
        public int so_luong { get; set; }
        public decimal don_gia { get; set; }
        public DateTime ngay_tao { get; set; }

        public GioHang? GioHang { get; set; }
        public SanPham? SanPham { get; set; }
        public BienThe? BienThe { get; set; }
    }
}