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

        /// <summary>
        /// Lấy thông tin chi tiết của đơn hàng theo mã đơn hàng,
        /// bao gồm người dùng, mã giảm giá và danh sách sản phẩm trong đơn hàng.
        /// </summary>
        /// <param name="id">Mã đơn hàng cần tìm.</param>
        /// <returns>Đối tượng hóa đơn cùng các thông tin liên quan.</returns>
        public async Task<HoaDon> GetOrderWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(h => h.NguoiDung)
                .Include(h => h.MaGiamGia)
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(c => c.SanPham)
                .FirstOrDefaultAsync(h => h.id_hoa_don == id);
        }


        /// <summary>
        /// Lấy danh sách đơn hàng của một người dùng,
        /// sắp xếp theo thời gian đặt mới nhất.
        /// </summary>
        /// <param name="userId">Mã người dùng.</param>
        /// <returns>Danh sách các đơn hàng của người dùng.</returns>
        public async Task<IEnumerable<HoaDon>> GetOrdersByUserAsync(int userId)
        {
            return await _dbSet
                .Include(h => h.ChiTietHoaDons)
                .Where(h => h.id_nguoi_dung == userId)
                .OrderByDescending(h => h.thoi_gian_dat)
                .ToListAsync();
        }

        /// <summary>
        /// Tạo mới đơn hàng và lưu các chi tiết đơn hàng tương ứng trong một giao dịch.
        /// Nếu xảy ra lỗi, toàn bộ thao tác sẽ được hoàn tác.
        /// </summary>
        /// <param name="hoaDon">Thông tin hóa đơn cần tạo.</param>
        /// <param name="chiTiets">Danh sách chi tiết hóa đơn.</param>
        /// <returns>Hóa đơn vừa được tạo.</returns>
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

        /// <summary>
        /// Cập nhật trạng thái của đơn hàng.
        /// Nếu trạng thái là "Hoàn thành" thì ghi nhận thời gian giao hàng thực tế.
        /// </summary>
        /// <param name="orderId">Mã đơn hàng cần cập nhật.</param>
        /// <param name="trangThai">Trạng thái mới của đơn hàng.</param>
        /// <returns>
        /// True nếu cập nhật thành công, False nếu không tìm thấy đơn hàng.
        /// </returns>
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