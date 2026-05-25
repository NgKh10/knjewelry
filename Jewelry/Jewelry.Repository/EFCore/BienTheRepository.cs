using Microsoft.EntityFrameworkCore;
using Jewelry.Entities;

namespace Jewelry.Repository.EFCore
{
    public interface IBienTheRepository : IRepository<BienThe>
    {
        Task<(List<BienThe> Items, int TotalCount)> SearchAsync(
            string? tenSanPham, string? kichCo, string? mauSac, int page, int pageSize, string? sortOrder = null);
        Task<List<BienThe>> GetBySanPhamAsync(int idSanPham);
    }

    public class BienTheRepository : Repository<BienThe>, IBienTheRepository
    {
        public BienTheRepository(AppDbContext context) : base(context) { }

        public async Task<(List<BienThe> Items, int TotalCount)> SearchAsync(
            string? tenSanPham, string? kichCo, string? mauSac, int page, int pageSize, string? sortOrder = null)
        {
            var query = _dbSet.Include(bt => bt.SanPham).AsQueryable();

            if (!string.IsNullOrEmpty(tenSanPham))
                query = query.Where(bt => bt.SanPham != null && bt.SanPham.ten_sp.ToLower().Contains(tenSanPham.ToLower()));

            if (!string.IsNullOrEmpty(kichCo))
                query = query.Where(bt => bt.kich_co != null && bt.kich_co.ToLower().Contains(kichCo.ToLower()));

            if (!string.IsNullOrEmpty(mauSac))
                query = query.Where(bt => bt.mau_sac != null && bt.mau_sac.ToLower().Contains(mauSac.ToLower()));

            var totalCount = await query.CountAsync();

            IQueryable<BienThe> orderedQuery = sortOrder == "desc"
                ? query.OrderByDescending(bt => bt.kich_co)
                : sortOrder == "asc"
                    ? query.OrderBy(bt => bt.kich_co)
                    : query.OrderBy(bt => bt.id_san_pham).ThenBy(bt => bt.id_bien_the);

            var items = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<BienThe>> GetBySanPhamAsync(int idSanPham)
        {
            return await _dbSet
                .Where(bt => bt.id_san_pham == idSanPham)
                .OrderBy(bt => bt.id_bien_the)
                .ToListAsync();
        }
    }
}
