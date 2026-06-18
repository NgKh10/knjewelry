using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;

namespace knjewelry.Services
{
    public interface ITaiKhoanService
    {
        Task<NguoiDung> DangNhapAsync(string tenDangNhap, string matKhau);
        Task<NguoiDung> DangKyAsync(DangKyViewModel model);
        Task<NguoiDung> GetThongTinAsync(int userId);
        Task<bool> CapNhatThongTinAsync(int userId, CapNhatThongTinViewModel model);
        Task<bool> DoiMatKhauAsync(int userId, string matKhauCu, string matKhauMoi);
    }
}