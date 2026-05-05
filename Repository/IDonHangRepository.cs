using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public interface IDonHangRepository : IGenericRepository<HoaDon>
    {
        Task<HoaDon> GetOrderWithDetailsAsync(int id);
        Task<IEnumerable<HoaDon>> GetOrdersByUserAsync(int userId);
        Task<HoaDon> CreateOrderAsync(HoaDon hoaDon, List<ChiTietHoaDon> chiTiets);
        Task<bool> UpdateOrderStatusAsync(int orderId, string trangThai);
    }
}