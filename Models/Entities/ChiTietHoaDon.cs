using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key]
        public int id_chi_tiet { get; set; }

        [Required]
        public int id_hoa_don { get; set; }

        [Required]
        public int id_san_pham { get; set; }

        public int? id_bien_the { get; set; }

        [Required]
        [MaxLength(200)]
        public string ten_sp_luu { get; set; }

        [MaxLength(100)]
        public string chat_lieu_luu { get; set; }

        [MaxLength(50)]
        public string mau_sac_luu { get; set; }

        [MaxLength(20)]
        public string kich_co_luu { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int so_luong { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal don_gia { get; set; }

        [ForeignKey("id_hoa_don")]
        public virtual HoaDon HoaDon { get; set; }

        [ForeignKey("id_san_pham")]
        public virtual SanPham SanPham { get; set; }

        [ForeignKey("id_bien_the")]
        public virtual BienThe BienThe { get; set; }
    }
}