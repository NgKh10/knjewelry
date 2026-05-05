using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace knjewelry.Models.Entities
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        public int id_hoa_don { get; set; }

        public string ma_hoa_don { get; set; }

        public int? id_nguoi_dung { get; set; }

        [Required]
        [MaxLength(150)]
        public string ho_ten { get; set; }

        [Required]
        [MaxLength(150)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MaxLength(15)]
        public string so_dien_thoai { get; set; }

        [Required]
        [MaxLength(100)]
        public string tinh_thanh_pho { get; set; }

        [Required]
        [MaxLength(100)]
        public string phuong_xa { get; set; }

        [Required]
        [MaxLength(200)]
        public string dia_chi_cu_the { get; set; }

        public DateTime thoi_gian_dat { get; set; } = DateTime.Now;

        public DateTime? thoi_gian_giao_dk { get; set; }

        public DateTime? thoi_gian_giao_tt { get; set; }

        [MaxLength(50)]
        public string phuong_thuc_tt { get; set; } = "Chuyển khoản QR";

        public int? id_ma_giam_gia { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal tien_hang { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        public decimal tien_giam { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        public decimal phi_van_chuyen { get; set; } = 0;

        [Column(TypeName = "decimal(18,0)")]
        public decimal tong_tien { get; set; } = 0;

        [MaxLength(30)]
        public string trang_thai { get; set; } = "Chờ xác nhận";

        [MaxLength(500)]
        public string ghi_chu { get; set; }

        public bool xuat_hoa_don { get; set; } = false;

        [ForeignKey("id_nguoi_dung")]
        public virtual NguoiDung NguoiDung { get; set; }

        [ForeignKey("id_ma_giam_gia")]
        public virtual MaGiamGia MaGiamGia { get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}