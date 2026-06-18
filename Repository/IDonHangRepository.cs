using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public interface IDonHangRepository : IGenericRepository<HoaDon>
    {
        Task<HoaDon> GetOrderWithDetailsAsync(int id); // Lấy thông tin chi tiết của đơn hàng theo mã đơn hàng,
        Task<IEnumerable<HoaDon>> GetOrdersByUserAsync(int userId); // Lấy danh sách đơn hàng của một người dùng.
        Task<HoaDon> CreateOrderAsync(HoaDon hoaDon, List<ChiTietHoaDon> chiTiets); // Tạo mới đơn hàng cùng danh sách chi tiết đơn hàng.
        Task<bool> UpdateOrderStatusAsync(int orderId, string trangThai); // Cập nhật trạng thái của đơn hàng.
    }
}