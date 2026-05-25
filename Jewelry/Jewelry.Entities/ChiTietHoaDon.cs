using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key]
        public int id_chi_tiet { get; set; }

        [ForeignKey("HoaDon")]
        public int id_hoa_don { get; set; }

        [ForeignKey("SanPham")]
        public int id_san_pham { get; set; }

        [ForeignKey("BienThe")]
        public int? id_bien_the { get; set; }
        public string ten_sp_luu { get; set; } = "";
        public string? chat_lieu_luu { get; set; }
        public string? mau_sac_luu { get; set; }
        public string? kich_co_luu { get; set; }
        public int so_luong { get; set; }
        public decimal don_gia { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal thanh_tien { get; set; }

        public SanPham? SanPham { get; set; }
        public HoaDon? HoaDon { get; set; }
        public BienThe? BienThe { get; set; }
    }
}