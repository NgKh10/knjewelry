using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("BienThe")]
    public class BienThe
    {
        [Key]
        public int id_bien_the { get; set; }

        [ForeignKey("SanPham")]
        public int id_san_pham { get; set; }
        public string? kich_co { get; set; }
        public string? mau_sac { get; set; }
        public int so_luong_ton { get; set; }
        public decimal gia_them { get; set; }
        public SanPham? SanPham { get; set; }
        public List<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new();
    }
}