using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        public int id_nguoi_dung { get; set; }

        [Required]
        [MaxLength(150)]
        public string ho_ten { get; set; } = "";

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string email { get; set; } = "";

        [Required]
        [MaxLength(150)]
        public string ten_dang_nhap { get; set; } = "";

        [Required]
        [MaxLength(255)]
        public string mat_khau { get; set; } = "";

        [MaxLength(15)]
        public string? so_dien_thoai { get; set; }  // Cho phép null

        [MaxLength(300)]
        public string? dia_chi { get; set; }  // Cho phép null

        [Required]
        [MaxLength(20)]
        public string vai_tro { get; set; } = "khach_hang";

        public byte trang_thai { get; set; } = 1;

        public DateTime ngay_tao { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<HoaDon> HoaDons { get; set; }
        public virtual ICollection<GioHang> GioHangs { get; set; }
    }
}