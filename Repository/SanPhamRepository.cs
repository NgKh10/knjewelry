using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public class SanPhamRepository : GenericRepository<SanPham>, ISanPhamRepository
    {
        public SanPhamRepository(TrangSucBacContext context) : base(context)
        {
        }

        public async Task<SanPham> GetProductWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.LoaiSanPham)
                .Include(p => p.ChatLieu)
                .Include(p => p.HinhAnhs.OrderBy(i => i.thu_tu))
                .Include(p => p.BienThes)
                .FirstOrDefaultAsync(p => p.id_san_pham == id && p.trang_thai == 1);
        }

        public async Task<IEnumerable<SanPham>> GetNewProductsAsync(int take)
        {
            return await _dbSet
                .Include(p => p.HinhAnhs.Where(i => i.la_chinh))
                .Where(p => p.trang_thai == 1)
                .OrderByDescending(p => p.ngay_tao)
                .Take(take)
                .ToListAsync() ?? new List<SanPham>();
        }
        public async Task<IEnumerable<SanPham>> GetHotProductsAsync(int take)
        {
            return await _dbSet
                .Include(p => p.HinhAnhs.Where(i => i.la_chinh))
                .Where(p => p.trang_thai == 1 && p.gia_khuyen_mai != null)
                .OrderByDescending(p => p.gia_khuyen_mai)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<SanPham>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Include(p => p.HinhAnhs.Where(i => i.la_chinh))
                .Where(p => p.id_loai_sp == categoryId && p.trang_thai == 1)
                .OrderByDescending(p => p.ngay_tao)
                .ToListAsync();
        }

        public async Task<IEnumerable<SanPham>> SearchProductsAsync(string keyword, int? categoryId)
        {
            var query = _dbSet
                .Include(p => p.HinhAnhs.Where(i => i.la_chinh))
                .Where(p => p.trang_thai == 1);

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(p => p.ten_sp.Contains(keyword));

            if (categoryId.HasValue)
                query = query.Where(p => p.id_loai_sp == categoryId.Value);

            return await query.OrderByDescending(p => p.ngay_tao).ToListAsync();
        }

        public async Task<int> GetStockAsync(int productId, int? variantId)
        {
            if (variantId.HasValue)
            {
                var variant = await _context.BienThes.FindAsync(variantId.Value);
                return variant?.so_luong_ton ?? 0;
            }
            else
            {
                var defaultVariant = await _context.BienThes
                    .FirstOrDefaultAsync(v => v.id_san_pham == productId && v.kich_co == null);
                return defaultVariant?.so_luong_ton ?? 0;
            }
        }
    }
}