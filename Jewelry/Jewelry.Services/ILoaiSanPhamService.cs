using Jewelry.Entities;

namespace Jewelry.Services
{
    public interface ILoaiSanPhamService
    {
        Task<(List<LoaiSanPham> Items, int TotalCount)> SearchAsync(
            string? keyword, int? idLoaiCha, int page, int pageSize);
        Task<LoaiSanPham?> GetByIdAsync(int id);
        Task<LoaiSanPham> CreateAsync(LoaiSanPham loaiSanPham);
        Task UpdateAsync(LoaiSanPham loaiSanPham);
        Task DeleteAsync(int id);
    }
}