using Jewelry.Entities;

namespace Jewelry.Services
{
    public interface IMaGiamGiaService
    {
        Task<(List<MaGiamGia> Items, int TotalCount)> SearchAsync(
            string? keyword, string? loaiGiam, byte? trangThai, int page, int pageSize, string? sortOrder = null);
        Task<MaGiamGia?> GetByIdAsync(int id);
        Task<MaGiamGia?> GetByCodeAsync(string code);
        Task<MaGiamGia> CreateAsync(MaGiamGia entity);
        Task UpdateAsync(int id, MaGiamGia entity);
        Task DeleteAsync(int id);
    }
}
