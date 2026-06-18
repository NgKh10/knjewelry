using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly TrangSucBacContext _context;
        /// <summary>
        /// Khởi tạo đối tượng GioHangRepository.
        /// </summary>
        /// <param name="context">DbContext dùng để thao tác với cơ sở dữ liệu.</param>
        public GioHangRepository(TrangSucBacContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy giỏ hàng theo mã phiên hoặc người dùng.
        /// Nếu chưa tồn tại thì tạo mới giỏ hàng.
        /// </summary>
        /// <param name="maPhien">Mã phiên làm việc.</param>
        /// <param name="idNguoiDung">Mã người dùng (nếu đã đăng nhập).</param>
        /// <returns>Đối tượng giỏ hàng.</returns>
        public async Task<GioHang> GetOrCreateCartAsync(string maPhien, int? idNguoiDung = null)
        {
            var cart = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .FirstOrDefaultAsync(g => g.ma_phien == maPhien || (idNguoiDung.HasValue && g.id_nguoi_dung == idNguoiDung));

            if (cart == null)
            {
                cart = new GioHang
                {
                    ma_phien = maPhien,
                    id_nguoi_dung = idNguoiDung,
                    ngay_tao = DateTime.Now,
                    ngay_cap_nhat = DateTime.Now
                };
                await _context.GioHangs.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        /// <summary>
        /// Lấy giỏ hàng theo mã phiên, bao gồm thông tin chi tiết sản phẩm và biến thể.
        /// </summary>
        /// <param name="maPhien">Mã phiên làm việc.</param>
        /// <returns>Đối tượng giỏ hàng.</returns>
        public async Task<GioHang> GetCartBySessionAsync(string maPhien)
        {
            return await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(c => c.SanPham)
                .ThenInclude(p => p.HinhAnhs)
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(c => c.BienThe)
                .FirstOrDefaultAsync(g => g.ma_phien == maPhien);
        }

        /// <summary>
        /// Lấy giỏ hàng của người dùng theo mã người dùng.
        /// </summary>
        /// <param name="idNguoiDung">Mã người dùng.</param>
        /// <returns>Đối tượng giỏ hàng.</returns>
        public async Task<GioHang> GetCartByUserAsync(int idNguoiDung)
        {
            return await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(c => c.SanPham)
                .ThenInclude(p => p.HinhAnhs)
                .Include(g => g.ChiTietGioHangs)
                .ThenInclude(c => c.BienThe)
                .FirstOrDefaultAsync(g => g.id_nguoi_dung == idNguoiDung);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng.
        /// Nếu sản phẩm đã tồn tại thì cập nhật số lượng.
        /// </summary>
        /// <param name="idGioHang">Mã giỏ hàng.</param>
        /// <param name="idSanPham">Mã sản phẩm.</param>
        /// <param name="idBienThe">Mã biến thể.</param>
        /// <param name="soLuong">Số lượng thêm vào.</param>
        /// <param name="donGia">Đơn giá sản phẩm.</param>
        public async Task AddToCartAsync(int idGioHang, int idSanPham, int? idBienThe, int soLuong, decimal donGia)
        {
            var existingItem = await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.id_gio_hang == idGioHang && c.id_san_pham == idSanPham && c.id_bien_the == idBienThe);

            if (existingItem != null)
            {
                existingItem.so_luong += soLuong;
                _context.ChiTietGioHangs.Update(existingItem);
            }
            else
            {
                var cartDetail = new ChiTietGioHang
                {
                    id_gio_hang = idGioHang,
                    id_san_pham = idSanPham,
                    id_bien_the = idBienThe,
                    so_luong = soLuong,
                    don_gia = donGia,
                    ngay_tao = DateTime.Now
                };
                await _context.ChiTietGioHangs.AddAsync(cartDetail);
            }

            var cart = await _context.GioHangs.FindAsync(idGioHang);
            if (cart != null)
            {
                cart.ngay_cap_nhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Cập nhật số lượng của một sản phẩm trong giỏ hàng.
        /// Nếu số lượng nhỏ hơn hoặc bằng 0 thì xóa sản phẩm khỏi giỏ.
        /// </summary>
        /// <param name="idChiTiet">Mã chi tiết giỏ hàng.</param>
        /// <param name="soLuong">Số lượng mới.</param>
        public async Task UpdateCartItemAsync(int idChiTiet, int soLuong)
        {
            var item = await _context.ChiTietGioHangs.FindAsync(idChiTiet);
            if (item != null)
            {
                if (soLuong <= 0)
                {
                    _context.ChiTietGioHangs.Remove(item);
                }
                else
                {
                    item.so_luong = soLuong;
                }
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Xóa một sản phẩm khỏi giỏ hàng.
        /// </summary>
        /// <param name="idChiTiet">Mã chi tiết giỏ hàng.</param>
        public async Task RemoveFromCartAsync(int idChiTiet)
        {
            var item = await _context.ChiTietGioHangs.FindAsync(idChiTiet);
            if (item != null)
            {
                _context.ChiTietGioHangs.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Xóa toàn bộ sản phẩm trong giỏ hàng.
        /// </summary>
        /// <param name="idGioHang">Mã giỏ hàng.</param>
        public async Task ClearCartAsync(int idGioHang)
        {
            var items = await _context.ChiTietGioHangs.Where(c => c.id_gio_hang == idGioHang).ToListAsync();
            _context.ChiTietGioHangs.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Lấy danh sách chi tiết sản phẩm trong giỏ hàng.
        /// </summary>
        /// <param name="idGioHang">Mã giỏ hàng.</param>
        /// <returns>Danh sách chi tiết giỏ hàng.</returns>
        public async Task<List<ChiTietGioHang>> GetCartDetailsAsync(int idGioHang)
        {
            return await _context.ChiTietGioHangs
                .Include(c => c.SanPham)
                .ThenInclude(p => p.HinhAnhs)
                .Include(c => c.BienThe)
                .Where(c => c.id_gio_hang == idGioHang)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy thông tin một sản phẩm cụ thể trong giỏ hàng.
        /// </summary>
        /// <param name="idGioHang">Mã giỏ hàng.</param>
        /// <param name="idSanPham">Mã sản phẩm.</param>
        /// <param name="idBienThe">Mã biến thể.</param>
        /// <returns>Chi tiết giỏ hàng tương ứng.</returns>
        public async Task<ChiTietGioHang> GetCartDetailAsync(int idGioHang, int idSanPham, int? idBienThe)
        {
            return await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.id_gio_hang == idGioHang && c.id_san_pham == idSanPham && c.id_bien_the == idBienThe);
        }

        /// <summary>
        /// Đếm tổng số lượng sản phẩm trong giỏ hàng.
        /// </summary>
        /// <param name="maPhien">Mã phiên làm việc.</param>
        /// <param name="idNguoiDung">Mã người dùng (nếu có).</param>
        /// <returns>Tổng số lượng sản phẩm trong giỏ hàng.</returns>
        public async Task<int> GetCartCountAsync(string maPhien, int? idNguoiDung = null)
        {
            var cart = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .FirstOrDefaultAsync(g => g.ma_phien == maPhien || (idNguoiDung.HasValue && g.id_nguoi_dung == idNguoiDung));

            return cart?.ChiTietGioHangs?.Sum(c => c.so_luong) ?? 0;
        }

        /// <summary>
        /// Gộp giỏ hàng của khách vãng lai với giỏ hàng của người dùng sau khi đăng nhập.
        /// Nếu người dùng chưa có giỏ hàng thì gán giỏ hàng hiện tại cho người dùng.
        /// </summary>
        /// <param name="maPhien">Mã phiên làm việc.</param>
        /// <param name="idNguoiDung">Mã người dùng.</param>
        public async Task MergeCartAsync(string maPhien, int idNguoiDung)
        {
            var sessionCart = await GetCartBySessionAsync(maPhien);
            var userCart = await GetCartByUserAsync(idNguoiDung);

            if (sessionCart == null) return;

            if (userCart == null)
            {
                sessionCart.id_nguoi_dung = idNguoiDung;
                sessionCart.ma_phien = maPhien;
                await _context.SaveChangesAsync();
            }
            else
            {
                foreach (var sessionItem in sessionCart.ChiTietGioHangs)
                {
                    var existingItem = userCart.ChiTietGioHangs
                        .FirstOrDefault(c => c.id_san_pham == sessionItem.id_san_pham && c.id_bien_the == sessionItem.id_bien_the);

                    if (existingItem != null)
                    {
                        existingItem.so_luong += sessionItem.so_luong;
                    }
                    else
                    {
                        sessionItem.id_gio_hang = userCart.id_gio_hang;
                        await _context.ChiTietGioHangs.AddAsync(sessionItem);
                    }
                }

                _context.GioHangs.Remove(sessionCart);
                await _context.SaveChangesAsync();
            }
        }
    }
}