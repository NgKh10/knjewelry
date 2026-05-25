using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        public int id_nguoi_dung { get; set; }
        public string ho_ten { get; set; }
        public string email { get; set; }
        public string ten_dang_nhap { get; set; }
        public string mat_khau { get; set; }
        public string? so_dien_thoai { get; set; }
        public string? dia_chi { get; set; }
        public string vai_tro { get; set; }  // 'khach_hang' hoặc 'quan_tri'
        public byte trang_thai { get; set; }
        public DateTime ngay_tao { get; set; }
        public List<HoaDon> HoaDons { get; set; } = new();
        public List<AdminLog> AdminLogs { get; set; } = new();
    }
}
