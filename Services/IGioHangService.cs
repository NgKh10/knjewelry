using knjewelry.Models.ViewModels;

namespace knjewelry.Services
{
    public interface IGioHangService
    {
        Task<GioHangViewModel> GetGioHangAsync();
        Task<int> GetSoLuongGioHangAsync();
        Task<bool> ThemVaoGioAsync(int sanPhamId, int? bienTheId, int soLuong);
        Task<bool> CapNhatSoLuongAsync(int sanPhamId, int? bienTheId, int soLuong);
        Task<bool> XoaKhoiGioAsync(int sanPhamId, int? bienTheId);
        Task<bool> XoaToanBoGioAsync();
        Task<bool> ApDungMaGiamGiaAsync(string maGiamGia);
        Task<bool> HopNhatGioHangSauDangNhapAsync(int userId);
    }
}