using Microsoft.EntityFrameworkCore;
using Jewelry.Entities;

namespace Jewelry.Repository.EFCore
{
    public interface IHinhAnhSanPhamRepository : IRepository<HinhAnhSanPham>
    {
        Task<(List<HinhAnhSanPham> Items, int TotalCount)> SearchAsync(
            int? idSanPham, bool? laChinhFilter, string? tenSanPham, int page, int pageSize, string? sortOrder = null);
        Task<List<HinhAnhSanPham>> GetBySanPhamAsync(int idSanPham);
    }

    public class HinhAnhSanPhamRepository : Repository<HinhAnhSanPham>, IHinhAnhSanPhamRepository
    {
        public HinhAnhSanPhamRepository(AppDbContext context) : base(context) { }

        public async Task<(List<HinhAnhSanPham> Items, int TotalCount)> SearchAsync(
            int? idSanPham, bool? laChinhFilter, string? tenSanPham, int page, int pageSize, string? sortOrder = null)
        {
            var query = _dbSet.Include(h => h.SanPham).AsQueryable();

            if (idSanPham.HasValue)
                query = query.Where(h => h.id_san_pham == idSanPham.Value);

            if (laChinhFilter.HasValue)
                query = query.Where(h => h.la_chinh == laChinhFilter.Value);

            if (!string.IsNullOrEmpty(tenSanPham))
                query = query.Where(h => h.SanPham != null && h.SanPham.ten_sp.ToLower().Contains(tenSanPham.ToLower()));

            var totalCount = await query.CountAsync();

            IQueryable<HinhAnhSanPham> orderedQuery = sortOrder == "desc"
                ? query.OrderByDescending(h => h.SanPham!.ten_sp)
                : sortOrder == "asc"
                    ? query.OrderBy(h => h.SanPham!.ten_sp)
                    : query.OrderBy(h => h.id_san_pham).ThenBy(h => h.thu_tu);

            var items = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<HinhAnhSanPham>> GetBySanPhamAsync(int idSanPham)
        {
            return await _dbSet
                .Where(h => h.id_san_pham == idSanPham)
                .OrderBy(h => h.thu_tu)
                .ToListAsync();
        }
    }
}
