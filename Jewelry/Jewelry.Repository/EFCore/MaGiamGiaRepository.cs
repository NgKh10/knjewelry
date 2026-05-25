using Microsoft.EntityFrameworkCore;
using Jewelry.Entities;

namespace Jewelry.Repository.EFCore
{
    public interface IMaGiamGiaRepository : IRepository<MaGiamGia>
    {
        Task<(List<MaGiamGia> Items, int TotalCount)> SearchAsync(
            string? keyword, string? loaiGiam, byte? trangThai, int page, int pageSize, string? sortOrder = null);
        Task<MaGiamGia?> GetByCodeAsync(string code);
        Task<bool> HasOrdersAsync(int id);
    }

    public class MaGiamGiaRepository : Repository<MaGiamGia>, IMaGiamGiaRepository
    {
        public MaGiamGiaRepository(AppDbContext context) : base(context) { }

        public async Task<(List<MaGiamGia> Items, int TotalCount)> SearchAsync(
            string? keyword, string? loaiGiam, byte? trangThai, int page, int pageSize, string? sortOrder = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(m =>
                    m.ma_code.ToLower().Contains(keyword) ||
                    (m.mo_ta != null && m.mo_ta.ToLower().Contains(keyword)));
            }

            if (!string.IsNullOrEmpty(loaiGiam))
                query = query.Where(m => m.loai_giam == loaiGiam);

            if (trangThai.HasValue)
                query = query.Where(m => m.trang_thai == trangThai.Value);

            var totalCount = await query.CountAsync();

            IQueryable<MaGiamGia> orderedQuery = sortOrder == "desc"
                ? query.OrderByDescending(m => m.ma_code)
                : sortOrder == "asc"
                    ? query.OrderBy(m => m.ma_code)
                    : query.OrderBy(m => m.ma_code);

            var items = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<MaGiamGia?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(m =>
                m.ma_code.ToLower() == code.ToLower().Trim());
        }

        public async Task<bool> HasOrdersAsync(int id)
        {
            return await _context.HoaDons.AnyAsync(h => h.id_ma_giam_gia == id);
        }
    }
}
