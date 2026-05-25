using Jewelry.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jewelry.Repository.EFCore
{
    public interface ISanPhamRepository : IRepository<SanPham>
    {
        Task<(List<SanPham> Products, int TotalCount)> SearchAsync(
            string? keyword,
            int? loaiId,
            decimal? giaTu,
            decimal? giaDen,
            int page,
            int pageSize,
            string? sortOrder = null);

        Task<List<SanPham>> GetByLoaiAsync(int loaiId);
        Task<SanPham?> GetWithDetailsAsync(int id);
        Task<bool> IsNameExistsAsync(string tenSp, int? excludeId = null);
    }

    public class SanPhamRepository : Repository<SanPham>, ISanPhamRepository
    {
        public SanPhamRepository(AppDbContext context) : base(context) { }

        public async Task<(List<SanPham> Products, int TotalCount)> SearchAsync(
            string? keyword,
            int? loaiId,
            decimal? giaTu,
            decimal? giaDen,
            int page,
            int pageSize,
            string? sortOrder = null)
        {
            var query = _dbSet
                .Include(p => p.LoaiSanPham)
                .Include(p => p.ChatLieu)
                .AsQueryable();

            // 🔍 Tiêu chí 1: Tìm theo tên hoặc mô tả
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower().Trim();
                query = query.Where(p =>
                    p.ten_sp.ToLower().Contains(keyword) ||
                    (p.mo_ta != null && p.mo_ta.ToLower().Contains(keyword)));
            }

            // 🔍 Tiêu chí 2: Lọc theo danh mục
            if (loaiId.HasValue && loaiId > 0)
            {
                query = query.Where(p => p.id_loai_sp == loaiId);
            }

         
            if (giaTu.HasValue)
            {
                query = query.Where(p => p.gia >= giaTu.Value);
            }
            if (giaDen.HasValue)
            {
                query = query.Where(p => p.gia <= giaDen.Value);
            }

            // Chỉ lấy sản phẩm đang bán (trang_thai = 1)
            query = query.Where(p => p.trang_thai == 1);

            var totalCount = await query.CountAsync();

            IQueryable<SanPham> orderedQuery = sortOrder == "desc"
                ? query.OrderByDescending(p => p.ten_sp)
                : sortOrder == "asc"
                    ? query.OrderBy(p => p.ten_sp)
                    : query.OrderBy(p => p.ten_sp);

            var products = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new SanPham
                {
                    id_san_pham = p.id_san_pham,
                    ten_sp = p.ten_sp,
                    gia = p.gia,
                    gia_khuyen_mai = p.gia_khuyen_mai,
                    id_loai_sp = p.id_loai_sp,
                    id_chat_lieu = p.id_chat_lieu,
                    trong_luong = p.trong_luong,
                    mo_ta = p.mo_ta,
                    trang_thai = p.trang_thai,
                    ngay_tao = p.ngay_tao,
                    LoaiSanPham = p.LoaiSanPham != null ? new LoaiSanPham
                    {
                        id_loai_sp = p.LoaiSanPham.id_loai_sp,
                        ten_loai = p.LoaiSanPham.ten_loai
                    } : null,
                    ChatLieu = p.ChatLieu != null ? new ChatLieu
                    {
                        id_chat_lieu = p.ChatLieu.id_chat_lieu,
                        ten_chat_lieu = p.ChatLieu.ten_chat_lieu
                    } : null
                })
                .ToListAsync();

            return (products, totalCount);
        }

        public async Task<List<SanPham>> GetByLoaiAsync(int loaiId)
        {
            return await _dbSet
                .Where(p => p.id_loai_sp == loaiId && p.trang_thai == 1)
                .OrderByDescending(p => p.ngay_tao)
                .ToListAsync();
        }

        public async Task<SanPham?> GetWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.LoaiSanPham)
                .Include(p => p.ChatLieu)
                .Include(p => p.BienThes)
                .Include(p => p.HinhAnhSanPhams)
                .FirstOrDefaultAsync(p => p.id_san_pham == id);
        }

        public async Task<bool> IsNameExistsAsync(string tenSp, int? excludeId = null)
        {
            var query = _dbSet.Where(p => p.ten_sp.ToLower() == tenSp.ToLower().Trim());
            if (excludeId.HasValue)
                query = query.Where(p => p.id_san_pham != excludeId.Value);
            return await query.AnyAsync();
        }
    }
}