using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;

namespace knjewelry.Services
{
    public interface ISanPhamService
    {
        Task<DanhSachSanPhamViewModel> GetDanhSachSanPhamAsync(int? loaiId, string search, int page, int pageSize = 12);
        Task<ChiTietSanPhamViewModel> GetChiTietSanPhamAsync(int id);
        Task<IEnumerable<SanPham>> GetSanPhamMoiAsync(int take);
        Task<IEnumerable<SanPham>> GetSanPhamHotAsync(int take);
    }
}