using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("MaGiamGia")]
    public class MaGiamGia
    {
        [Key]
        public int id_ma_giam_gia { get; set; }

        [Required]
        [MaxLength(50)]
        public string ma_code { get; set; }

        [MaxLength(300)]
        public string mo_ta { get; set; }

        [Required]
        [MaxLength(10)]
        public string loai_giam { get; set; } // phan_tram hoac tien_mat

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal gia_tri { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal? giam_toi_da { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal don_hang_toi_thieu { get; set; } = 0;

        public int? so_luong { get; set; }

        public int da_su_dung { get; set; } = 0;

        public DateTime? ngay_bat_dau { get; set; }

        public DateTime? ngay_ket_thuc { get; set; }

        public byte trang_thai { get; set; } = 1;

        public virtual ICollection<HoaDon> HoaDons { get; set; }
    }
}