using Jewelry.Entities;

using Microsoft.EntityFrameworkCore;

namespace Jewelry.Repository.EFCore
{
    public interface ILoaiSanPhamRepository : IRepository<LoaiSanPham>
    {
        Task<(List<LoaiSanPham> Items, int TotalCount)> SearchAsync(
            string? keyword,
            int? idLoaiCha,
            int page,
            int pageSize,
            string? sortOrder = null);

        Task<LoaiSanPham?> GetByNameAsync(string name);
        Task<bool> HasChildrenAsync(int id);
        Task<bool> HasProductsAsync(int id);
    }

    public class LoaiSanPhamRepository : Repository<LoaiSanPham>, ILoaiSanPhamRepository
    {
        public LoaiSanPhamRepository(AppDbContext context) : base(context) { }

        public async Task<(List<LoaiSanPham> Items, int TotalCount)> SearchAsync(
            string? keyword,
            int? idLoaiCha,
            int page,
            int pageSize,
            string? sortOrder = null)
        {
            var query = _dbSet.AsQueryable();

            // Tìm theo từ khóa
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower().Trim();
                query = query.Where(l => l.ten_loai.ToLower().Contains(keyword));
            }

            // Lọc theo danh mục cha
            if (idLoaiCha.HasValue)
            {
                if (idLoaiCha.Value == 0)
                    query = query.Where(l => l.id_loai_cha == null);
                else
                    query = query.Where(l => l.id_loai_cha == idLoaiCha.Value);
            }

            var totalCount = await query.CountAsync();

            IQueryable<LoaiSanPham> orderedQuery = sortOrder == "desc"
                ? query.OrderByDescending(l => l.ten_loai)
                : sortOrder == "asc"
                    ? query.OrderBy(l => l.ten_loai)
                    : query.OrderBy(l => l.id_loai_cha).ThenBy(l => l.thu_tu);

            var items = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LoaiSanPham
                {
                    id_loai_sp = l.id_loai_sp,
                    ten_loai = l.ten_loai,
                    id_loai_cha = l.id_loai_cha,
                    thu_tu = l.thu_tu,
                    mo_ta = l.mo_ta
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<LoaiSanPham?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(l => l.ten_loai.ToLower() == name.ToLower());
        }

        public async Task<bool> HasChildrenAsync(int id)
        {
            return await _dbSet.AnyAsync(l => l.id_loai_cha == id);
        }

        public async Task<bool> HasProductsAsync(int id)
        {
            return await _context.SanPhams.AnyAsync(sp => sp.id_loai_sp == id);
        }
    }
}