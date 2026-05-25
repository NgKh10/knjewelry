using Jewelry.Entities;

namespace Jewelry.Services
{
    public interface IBienTheService
    {
        Task<(List<BienThe> Items, int TotalCount)> SearchAsync(
            string? tenSanPham, string? kichCo, string? mauSac, int page, int pageSize, string? sortOrder = null);
        Task<BienThe?> GetByIdAsync(int id);
        Task<List<BienThe>> GetBySanPhamAsync(int idSanPham);
        Task<BienThe> CreateAsync(BienThe entity);
        Task UpdateAsync(int id, BienThe entity);
        Task DeleteAsync(int id);
    }
}
