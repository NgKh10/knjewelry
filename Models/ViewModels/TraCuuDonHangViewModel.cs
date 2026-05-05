using System.ComponentModel.DataAnnotations;
using knjewelry.Models.Entities;

namespace knjewelry.Models.ViewModels
{
    public class TraCuuDonHangViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã đơn hàng")]
        [Display(Name = "Mã đơn hàng")]
        [RegularExpression(@"^HD\d{6}$", ErrorMessage = "Mã đơn hàng không hợp lệ (VD: HD000001)")]
        public string MaDonHang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class KetQuaTraCuuViewModel
    {
        public HoaDon DonHang { get; set; }
        public List<LichSuTrangThaiViewModel> LichSuTrangThai { get; set; }
    }

    public class LichSuTrangThaiViewModel
    {
        public string TrangThai { get; set; }
        public DateTime ThoiGian { get; set; }
        public string GhiChu { get; set; }
        public bool DaHoanThanh { get; set; }

        public string TenTrangThai => TrangThai switch
        {
            "Chờ xác nhận" => "Đơn hàng đã được tạo",
            "Đã xác nhận" => "Đơn hàng đã được xác nhận",
            "Đang giao" => "Đơn hàng đang được giao",
            "Hoàn thành" => "Đơn hàng đã giao thành công",
            "Hủy" => "Đơn hàng đã bị hủy",
            _ => TrangThai
        };
    }
}