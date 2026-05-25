using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("GioHang")]
    public class GioHang
    {
        [Key]
        public int id_gio_hang { get; set; }
        public string ma_phien { get; set; } = "";

        [ForeignKey("NguoiDung")]
        public int? id_nguoi_dung { get; set; }
        public DateTime ngay_tao { get; set; }
        public DateTime ngay_cap_nhat { get; set; }

        public NguoiDung? NguoiDung { get; set; }
        public List<ChiTietGioHang> ChiTietGioHangs { get; set; } = new();
    }
}