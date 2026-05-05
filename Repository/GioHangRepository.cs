using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly TrangSucBacContext _context;

        public GioHangRepository(TrangSucBacContext context)
        {
            _context = context;
        }

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

        public async Task RemoveFromCartAsync(int idChiTiet)
        {
            var item = await _context.ChiTietGioHangs.FindAsync(idChiTiet);
            if (item != null)
            {
                _context.ChiTietGioHangs.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int idGioHang)
        {
            var items = await _context.ChiTietGioHangs.Where(c => c.id_gio_hang == idGioHang).ToListAsync();
            _context.ChiTietGioHangs.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChiTietGioHang>> GetCartDetailsAsync(int idGioHang)
        {
            return await _context.ChiTietGioHangs
                .Include(c => c.SanPham)
                .ThenInclude(p => p.HinhAnhs)
                .Include(c => c.BienThe)
                .Where(c => c.id_gio_hang == idGioHang)
                .ToListAsync();
        }

        public async Task<ChiTietGioHang> GetCartDetailAsync(int idGioHang, int idSanPham, int? idBienThe)
        {
            return await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.id_gio_hang == idGioHang && c.id_san_pham == idSanPham && c.id_bien_the == idBienThe);
        }

        public async Task<int> GetCartCountAsync(string maPhien, int? idNguoiDung = null)
        {
            var cart = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .FirstOrDefaultAsync(g => g.ma_phien == maPhien || (idNguoiDung.HasValue && g.id_nguoi_dung == idNguoiDung));

            return cart?.ChiTietGioHangs?.Sum(c => c.so_luong) ?? 0;
        }

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