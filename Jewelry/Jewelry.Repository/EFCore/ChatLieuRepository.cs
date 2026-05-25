using Microsoft.EntityFrameworkCore;
using Jewelry.Entities;

namespace Jewelry.Repository.EFCore
{
    /// <summary>
    /// ChatLieu Repository Interface
    /// </summary>
    public interface IChatLieuRepository : IRepository<ChatLieu>
    {
        Task<(List<ChatLieu> Items, int TotalCount)> SearchAsync(
            string? keyword, string? doTinhKhiet, int page, int pageSize, string? sortOrder = null);
        Task<ChatLieu?> GetByNameAsync(string name);
        Task<bool> HasProductsAsync(int id);
    }

    /// <summary>
    /// ChatLieu Repository Implementation
    /// </summary>
    public class ChatLieuRepository : Repository<ChatLieu>, IChatLieuRepository
    {
        public ChatLieuRepository(AppDbContext context) : base(context) { }

        public async Task<(List<ChatLieu> Items, int TotalCount)> SearchAsync(
            string? keyword, string? doTinhKhiet, int page, int pageSize, string? sortOrder = null)
        {
            var query = _dbSet.AsQueryable();

            // Tìm theo tên chất liệu
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower().Trim();
                query = query.Where(c => c.ten_chat_lieu.ToLower().Contains(keyword));
            }

            // Lọc theo độ tinh khiết
            if (!string.IsNullOrEmpty(doTinhKhiet))
            {
                doTinhKhiet = doTinhKhiet.ToLower().Trim();
                query = query.Where(c =>
                    c.do_tinh_khiet != null && c.do_tinh_khiet.ToLower().Contains(doTinhKhiet));
            }

            var totalCount = await query.CountAsync();

            IQueryable<ChatLieu> orderedQuery = sortOrder == "desc"
                ? query.OrderByDescending(c => c.ten_chat_lieu)
                : sortOrder == "asc"
                    ? query.OrderBy(c => c.ten_chat_lieu)
                    : query.OrderBy(c => c.ten_chat_lieu);

            var items = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ChatLieu
                {
                    id_chat_lieu = c.id_chat_lieu,
                    ten_chat_lieu = c.ten_chat_lieu,
                    do_tinh_khiet = c.do_tinh_khiet,
                    mo_ta = c.mo_ta
                })
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<ChatLieu?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c =>
                c.ten_chat_lieu.ToLower() == name.ToLower().Trim());
        }

        public async Task<bool> HasProductsAsync(int id)
        {
            return await _context.SanPhams.AnyAsync(sp => sp.id_chat_lieu == id);
        }
    }
}