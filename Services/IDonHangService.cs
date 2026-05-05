using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;

namespace knjewelry.Services
{
    public interface IDonHangService
    {
        Task<HoaDon> TaoDonHangAsync(ThanhToanViewModel model, int? userId);
        Task<HoaDon> GetDonHangByIdAsync(int id);
        Task<HoaDon> GetDonHangChiTietAsync(int id);
        Task<IEnumerable<HoaDon>> GetDonHangTheoNguoiDungAsync(int userId);
        Task<bool> CapNhatTrangThaiDonHangAsync(int donHangId, string trangThai);
    }
}