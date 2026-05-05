using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        public int id_san_pham { get; set; }

        [Required]
        [MaxLength(200)]
        public string ten_sp { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal gia { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal? gia_khuyen_mai { get; set; }

        [Required]
        public int id_loai_sp { get; set; }

        [Required]
        public int id_chat_lieu { get; set; }

        [Column(TypeName = "decimal(8,2)")]
        public decimal? trong_luong { get; set; }

        public string mo_ta { get; set; }

        public byte trang_thai { get; set; } = 1;

        public DateTime ngay_tao { get; set; } = DateTime.Now;

        [ForeignKey("id_loai_sp")]
        public virtual LoaiSanPham LoaiSanPham { get; set; }

        [ForeignKey("id_chat_lieu")]
        public virtual ChatLieu ChatLieu { get; set; }

        public virtual ICollection<BienThe> BienThes { get; set; }
        public virtual ICollection<HinhAnhSanPham> HinhAnhs { get; set; }
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; }
    }
}