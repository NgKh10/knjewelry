using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public interface IGioHangRepository
    {
        Task<GioHang> GetOrCreateCartAsync(string maPhien, int? idNguoiDung = null);
        Task<GioHang> GetCartBySessionAsync(string maPhien);
        Task<GioHang> GetCartByUserAsync(int idNguoiDung);
        Task AddToCartAsync(int idGioHang, int idSanPham, int? idBienThe, int soLuong, decimal donGia);
        Task UpdateCartItemAsync(int idChiTiet, int soLuong);
        Task RemoveFromCartAsync(int idChiTiet);
        Task ClearCartAsync(int idGioHang);
        Task<List<ChiTietGioHang>> GetCartDetailsAsync(int idGioHang);
        Task<ChiTietGioHang> GetCartDetailAsync(int idGioHang, int idSanPham, int? idBienThe);
        Task<int> GetCartCountAsync(string maPhien, int? idNguoiDung = null);
        Task MergeCartAsync(string maPhien, int idNguoiDung);
    }
}