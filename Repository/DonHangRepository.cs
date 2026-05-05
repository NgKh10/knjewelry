using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public class DonHangRepository : GenericRepository<HoaDon>, IDonHangRepository
    {
        public DonHangRepository(TrangSucBacContext context) : base(context)
        {
        }

        public async Task<HoaDon> GetOrderWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(h => h.NguoiDung)
                .Include(h => h.MaGiamGia)
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(h => h.id_hoa_don == id);
        }

        public async Task<IEnumerable<HoaDon>> GetOrdersByUserAsync(int userId)
        {
            return await _dbSet
                .Include(h => h.ChiTietHoaDons)
                .Where(h => h.id_nguoi_dung == userId)
                .OrderByDescending(h => h.thoi_gian_dat)
                .ToListAsync();
        }

        public async Task<HoaDon> CreateOrderAsync(HoaDon hoaDon, List<ChiTietHoaDon> chiTiets)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _dbSet.AddAsync(hoaDon);
                await _context.SaveChangesAsync();

                foreach (var detail in chiTiets)
                {
                    detail.id_hoa_don = hoaDon.id_hoa_don;
                    await _context.ChiTietHoaDons.AddAsync(detail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return hoaDon;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string trangThai)
        {
            var order = await GetByIdAsync(orderId);
            if (order == null) return false;

            order.trang_thai = trangThai;
            if (trangThai == "Hoàn thành")
            {
                order.thoi_gian_giao_tt = DateTime.Now;
            }

            Update(order);
            await SaveChangesAsync();
            return true;
        }
    }
}