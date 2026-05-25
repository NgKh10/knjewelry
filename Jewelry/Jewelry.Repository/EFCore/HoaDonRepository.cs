using Microsoft.EntityFrameworkCore;
using Jewelry.Entities;

namespace Jewelry.Repository.EFCore
{
    /// <summary>
    /// HoaDon Repository using Entity Framework Core
    /// </summary>
    public interface IHoaDonRepository : IRepository<HoaDon>
    {
        Task<List<HoaDon>> GetByUserAsync(int userId);
        Task<HoaDon?> GetWithDetailsAsync(int orderId);
        Task<(List<HoaDon> Items, int TotalCount)> SearchAsync(
            string? keyword, string? trangThai, DateTime? tuNgay, DateTime? denNgay,
            int? idNguoiDung, int page, int pageSize, string? sortOrder = null);
    }

    public class HoaDonRepository : Repository<HoaDon>, IHoaDonRepository
    {
        public HoaDonRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<HoaDon>> GetByUserAsync(int userId)
        {
            return await _dbSet
                .Where(o => o.id_nguoi_dung == userId)
                .OrderByDescending(o => o.thoi_gian_dat)
                .ToListAsync();
        }

        public async Task<HoaDon?> GetWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.NguoiDung)
                .Include(o => o.MaGiamGia)
                .Include(o => o.ChiTietHoaDons)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(o => o.id_hoa_don == orderId);
        }

        public async Task<(List<HoaDon> Items, int TotalCount)> SearchAsync(
            string? keyword, string? trangThai, DateTime? tuNgay, DateTime? denNgay,
            int? idNguoiDung, int page, int pageSize, string? sortOrder = null)
        {
            var query = _dbSet.Include(o => o.NguoiDung).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(o =>
                    (o.ma_hoa_don != null && o.ma_hoa_don.ToLower().Contains(keyword)) ||
                    o.ho_ten.ToLower().Contains(keyword) ||
                    o.email.ToLower().Contains(keyword) ||
                    o.so_dien_thoai.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(o => o.trang_thai == trangThai);

            if (tuNgay.HasValue)
                query = query.Where(o => o.thoi_gian_dat >= tuNgay.Value);

            if (denNgay.HasValue)
                query = query.Where(o => o.thoi_gian_dat <= denNgay.Value.AddDays(1));

            if (idNguoiDung.HasValue)
                query = query.Where(o => o.id_nguoi_dung == idNguoiDung.Value);

            var totalCount = await query.CountAsync();

            IQueryable<HoaDon> orderedQuery = sortOrder == "desc"
                ? query.OrderByDescending(o => o.ho_ten)
                : sortOrder == "asc"
                    ? query.OrderBy(o => o.ho_ten)
                    : query.OrderByDescending(o => o.thoi_gian_dat);

            var items = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }

    /// <summary>
    /// ChiTietHoaDon Repository using Entity Framework Core
    /// </summary>
    public interface IChiTietHoaDonRepository : IRepository<ChiTietHoaDon>
    {
        Task<List<ChiTietHoaDon>> GetByOrderAsync(int orderId);
    }

    public class ChiTietHoaDonRepository : Repository<ChiTietHoaDon>, IChiTietHoaDonRepository
    {
        public ChiTietHoaDonRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<ChiTietHoaDon>> GetByOrderAsync(int orderId)
        {
            return await _dbSet
                .Where(od => od.id_hoa_don == orderId)
                .Include(od => od.SanPham)
                .ToListAsync();
        }
    }
}