using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jewelry.Entities
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        public int id_hoa_don { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string? ma_hoa_don { get; set; }

        [ForeignKey("NguoiDung")]
        public int? id_nguoi_dung { get; set; }
        public string ho_ten { get; set; } = "";
        public string email { get; set; } = "";
        public string so_dien_thoai { get; set; } = "";
        public string tinh_thanh_pho { get; set; } = "";
        public string phuong_xa { get; set; } = "";
        public string dia_chi_cu_the { get; set; } = "";
        public DateTime thoi_gian_dat { get; set; }
        public DateTime? thoi_gian_giao_dk { get; set; }
        public DateTime? thoi_gian_giao_tt { get; set; }
        public string phuong_thuc_tt { get; set; } = "";

        [ForeignKey("MaGiamGia")]
        public int? id_ma_giam_gia { get; set; }
        public decimal tien_hang { get; set; }
        public decimal tien_giam { get; set; }
        public decimal phi_van_chuyen { get; set; }
        public decimal tong_tien { get; set; }
        public string trang_thai { get; set; } = "";
        public string? ghi_chu { get; set; }
        public bool xuat_hoa_don { get; set; }

        public NguoiDung? NguoiDung { get; set; }
        public MaGiamGia? MaGiamGia { get; set; }
        public List<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}