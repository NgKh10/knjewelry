using Jewelry.Entities;

namespace Jewelry.Services
{
    public interface ISanPhamService
    {
        Task<(List<SanPham> Products, int TotalCount)> SearchAsync(
            string? keyword, int? loaiId, decimal? giaTu, decimal? giaDen, int page, int pageSize, string? sortOrder = null);
        Task<SanPham?> GetByIdAsync(int id);
        Task<SanPham> CreateAsync(SanPham sanPham);
        Task UpdateAsync(SanPham sanPham);
        Task DeleteAsync(int id);
    }
}