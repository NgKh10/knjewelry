using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("MaGiamGia")]
    public class MaGiamGia
    {
        [Key]
        public int id_ma_giam_gia { get; set; }
        public string ma_code { get; set; } = "";
        public string? mo_ta { get; set; }
        public string loai_giam { get; set; } = "";
        public decimal gia_tri { get; set; }
        public decimal? giam_toi_da { get; set; }
        public decimal don_hang_toi_thieu { get; set; }
        public int? so_luong { get; set; }
        public int da_su_dung { get; set; }
        public DateTime? ngay_bat_dau { get; set; }
        public DateTime? ngay_ket_thuc { get; set; }
        public byte trang_thai { get; set; }
    }
}