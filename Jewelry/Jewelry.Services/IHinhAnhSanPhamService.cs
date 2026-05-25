using Jewelry.Entities;

namespace Jewelry.Services
{
    public interface IHinhAnhSanPhamService
    {
        Task<(List<HinhAnhSanPham> Items, int TotalCount)> SearchAsync(
            int? idSanPham, bool? laChinhFilter, string? tenSanPham, int page, int pageSize, string? sortOrder = null);
        Task<HinhAnhSanPham?> GetByIdAsync(int id);
        Task<List<HinhAnhSanPham>> GetBySanPhamAsync(int idSanPham);
        Task<HinhAnhSanPham> CreateAsync(HinhAnhSanPham entity);
        Task UpdateAsync(int id, HinhAnhSanPham entity);
        Task SetMainAsync(int id);
        Task DeleteAsync(int id);
    }
}
