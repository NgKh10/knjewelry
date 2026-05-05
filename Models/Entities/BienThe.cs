using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("BienThe")]
    public class BienThe
    {
        [Key]
        public int id_bien_the { get; set; }

        [Required]
        public int id_san_pham { get; set; }

        [MaxLength(20)]
        public string kich_co { get; set; }

        [MaxLength(50)]
        public string mau_sac { get; set; }

        public int so_luong_ton { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        public decimal gia_them { get; set; } = 0;

        [ForeignKey("id_san_pham")]
        public virtual SanPham SanPham { get; set; }

        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}