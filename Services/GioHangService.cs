using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;
using knjewelry.Repository;

namespace knjewelry.Services
{
    public class GioHangService : IGioHangService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGioHangRepository _gioHangRepository;
        private readonly ISanPhamRepository _sanPhamRepository;
        private readonly IGenericRepository<MaGiamGia> _maGiamGiaRepository;

        public GioHangService(
            IHttpContextAccessor httpContextAccessor,
            IGioHangRepository gioHangRepository,
            ISanPhamRepository sanPhamRepository,
            IGenericRepository<MaGiamGia> maGiamGiaRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _gioHangRepository = gioHangRepository;
            _sanPhamRepository = sanPhamRepository;
            _maGiamGiaRepository = maGiamGiaRepository;
        }

        private string GetMaPhien()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var maPhien = session.GetString("MaPhien");
            if (string.IsNullOrEmpty(maPhien))
            {
                maPhien = Guid.NewGuid().ToString();
                session.SetString("MaPhien", maPhien);
            }
            return maPhien;
        }

        private int? GetUserId()
        {
            return _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
        }

        public async Task<GioHangViewModel> GetGioHangAsync()
        {
            var maPhien = GetMaPhien();
            var userId = GetUserId();
            var cart = await _gioHangRepository.GetOrCreateCartAsync(maPhien, userId);
            var cartDetails = await _gioHangRepository.GetCartDetailsAsync(cart.id_gio_hang);

            var viewModel = new GioHangViewModel();
            foreach (var item in cartDetails)
            {
                var sanPham = item.SanPham;
                var bienThe = item.BienThe;

                viewModel.DanhSachSanPham.Add(new GioHangItemViewModel
                {
                    IdChiTietGioHang = item.id_chi_tiet_gh,
                    IdSanPham = item.id_san_pham,
                    TenSanPham = sanPham.ten_sp,
                    DuongDanAnh = sanPham.HinhAnhs?.FirstOrDefault(i => i.la_chinh)?.duong_dan ?? "/images/default.jpg",
                    IdBienThe = item.id_bien_the,
                    KichCo = bienThe?.kich_co,
                    MauSac = bienThe?.mau_sac,
                    SoLuong = item.so_luong,
                    DonGia = item.don_gia,
                    ThanhTien = item.don_gia * item.so_luong,
                    TonKho = bienThe?.so_luong_ton ?? 0
                });
            }

            viewModel.TinhTong();
            return viewModel;
        }

        public async Task<int> GetSoLuongGioHangAsync()
        {
            var maPhien = GetMaPhien();
            var userId = GetUserId();
            return await _gioHangRepository.GetCartCountAsync(maPhien, userId);
        }

        public async Task<bool> ThemVaoGioAsync(int sanPhamId, int? bienTheId, int soLuong)
        {
            var sanPham = await _sanPhamRepository.GetProductWithDetailsAsync(sanPhamId);
            if (sanPham == null) return false;

            var tonKho = await _sanPhamRepository.GetStockAsync(sanPhamId, bienTheId);
            if (tonKho < soLuong) return false;

            var donGia = sanPham.gia_khuyen_mai ?? sanPham.gia;
            var maPhien = GetMaPhien();
            var userId = GetUserId();
            var cart = await _gioHangRepository.GetOrCreateCartAsync(maPhien, userId);

            await _gioHangRepository.AddToCartAsync(cart.id_gio_hang, sanPhamId, bienTheId, soLuong, donGia);

            var count = await GetSoLuongGioHangAsync();
            _httpContextAccessor.HttpContext.Session.SetCartSummary(count, 0);

            return true;
        }

        public async Task<bool> CapNhatSoLuongAsync(int sanPhamId, int? bienTheId, int soLuong)
        {
            var maPhien = GetMaPhien();
            var userId = GetUserId();
            var cart = await _gioHangRepository.GetCartBySessionAsync(maPhien);
            if (cart == null && userId.HasValue)
                cart = await _gioHangRepository.GetCartByUserAsync(userId.Value);

            if (cart == null) return false;

            var cartDetail = await _gioHangRepository.GetCartDetailAsync(cart.id_gio_hang, sanPhamId, bienTheId);
            if (cartDetail == null) return false;

            if (soLuong <= 0)
            {
                await _gioHangRepository.RemoveFromCartAsync(cartDetail.id_chi_tiet_gh);
            }
            else
            {
                await _gioHangRepository.UpdateCartItemAsync(cartDetail.id_chi_tiet_gh, soLuong);
            }

            var count = await GetSoLuongGioHangAsync();
            _httpContextAccessor.HttpContext.Session.SetCartSummary(count, 0);

            return true;
        }

        public async Task<bool> XoaKhoiGioAsync(int sanPhamId, int? bienTheId)
        {
            return await CapNhatSoLuongAsync(sanPhamId, bienTheId, 0);
        }

        public async Task<bool> XoaToanBoGioAsync()
        {
            var maPhien = GetMaPhien();
            var userId = GetUserId();
            var cart = await _gioHangRepository.GetCartBySessionAsync(maPhien);
            if (cart == null && userId.HasValue)
                cart = await _gioHangRepository.GetCartByUserAsync(userId.Value);

            if (cart == null) return false;

            await _gioHangRepository.ClearCartAsync(cart.id_gio_hang);
            _httpContextAccessor.HttpContext.Session.ClearCartSummary();

            return true;
        }

        public async Task<bool> ApDungMaGiamGiaAsync(string maGiamGia)
        {
            var discount = await _maGiamGiaRepository.SingleOrDefaultAsync(m =>
                m.ma_code == maGiamGia &&
                m.trang_thai == 1 &&
                (m.so_luong == null || m.da_su_dung < m.so_luong) &&
                (m.ngay_ket_thuc == null || m.ngay_ket_thuc >= DateTime.Now));

            if (discount == null) return false;

            var cart = await GetGioHangAsync();
            if (cart.TongTienHang < discount.don_hang_toi_thieu) return false;

            _httpContextAccessor.HttpContext.Session.SetString("AppliedDiscount", maGiamGia);
            return true;
        }

        public async Task<bool> HopNhatGioHangSauDangNhapAsync(int userId)
        {
            var maPhien = GetMaPhien();
            await _gioHangRepository.MergeCartAsync(maPhien, userId);
            var count = await GetSoLuongGioHangAsync();
            _httpContextAccessor.HttpContext.Session.SetInt32("CartCount", count);
            return true;
        }
    }
}