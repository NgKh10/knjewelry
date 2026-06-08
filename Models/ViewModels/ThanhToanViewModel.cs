using System.ComponentModel.DataAnnotations;
using knjewelry.Models.ViewModels;

namespace knjewelry.Models.ViewModels
{
    public class ThanhToanViewModel
    {
        public GioHangViewModel? GioHang { get; set; }  // loaded server-side, not in form

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên từ 2-150 ký tự")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố")]
        [Display(Name = "Tỉnh/Thành phố")]
        public string TinhTP { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn quận/huyện")]
        [Display(Name = "Quận/Huyện")]
        public string QuanHuyen { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phường/xã")]
        [Display(Name = "Phường/Xã")]
        public string PhuongXa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ cụ thể")]
        [Display(Name = "Địa chỉ cụ thể")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Địa chỉ từ 5-200 ký tự")]
        public string DiaChiCuThe { get; set; }

        public string DiaChiDayDu => $"{DiaChiCuThe}, {PhuongXa}, {QuanHuyen}, {TinhTP}";

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = "Chuyển khoản QR";

        [Display(Name = "Mã giảm giá")]
        public string? MaGiamGia { get; set; }  // optional

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        public string? GhiChu { get; set; }  // optional
    }

    public class ThanhToanQRViewModel
    {
        public string MaNganHang { get; set; }
        public string TenNganHang { get; set; }
        public string SoTaiKhoan { get; set; }
        public string TenTaiKhoan { get; set; }
        public decimal SoTien { get; set; }
        public string MaDonHang { get; set; }
        public string DuongDanQR { get; set; }
    }

    public class KetQuaThanhToanViewModel
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; }
        public int? HoaDonId { get; set; }
        public string MaDonHang { get; set; }
        public decimal SoTien { get; set; }
        public string PhuongThucThanhToan { get; set; }
        public DateTime ThoiGian { get; set; }
    }
}