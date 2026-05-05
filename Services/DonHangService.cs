using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;
using knjewelry.Repository;

namespace knjewelry.Services
{
    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _donHangRepository;
        private readonly ISanPhamRepository _sanPhamRepository;
        private readonly IGenericRepository<MaGiamGia> _maGiamGiaRepository;
        private readonly IGioHangService _gioHangService;

        public DonHangService(
            IDonHangRepository donHangRepository,
            ISanPhamRepository sanPhamRepository,
            IGenericRepository<MaGiamGia> maGiamGiaRepository,
            IGioHangService gioHangService)
        {
            _donHangRepository = donHangRepository;
            _sanPhamRepository = sanPhamRepository;
            _maGiamGiaRepository = maGiamGiaRepository;
            _gioHangService = gioHangService;
        }

        public async Task<HoaDon> TaoDonHangAsync(ThanhToanViewModel model, int? userId)
        {
            var cart = await _gioHangService.GetGioHangAsync();
            if (cart == null || !cart.DanhSachSanPham.Any())
                throw new Exception("Giỏ hàng trống");

            // Tính giảm giá
            decimal tienGiam = 0;
            MaGiamGia discount = null;

            if (!string.IsNullOrEmpty(cart.MaGiamGia))
            {
                discount = await _maGiamGiaRepository.SingleOrDefaultAsync(m =>
                    m.ma_code == cart.MaGiamGia && m.trang_thai == 1);

                if (discount != null && cart.TongTienHang >= discount.don_hang_toi_thieu)
                {
                    if (discount.loai_giam == "phan_tram")
                    {
                        tienGiam = cart.TongTienHang * discount.gia_tri / 100;
                        if (discount.giam_toi_da.HasValue && tienGiam > discount.giam_toi_da.Value)
                            tienGiam = discount.giam_toi_da.Value;
                    }
                    else
                    {
                        tienGiam = discount.gia_tri;
                    }
                }
            }

            // Tạo hóa đơn
            var hoaDon = new HoaDon
            {
                id_nguoi_dung = userId,
                ho_ten = model.HoTen,
                email = model.Email,
                so_dien_thoai = model.SoDienThoai,
                tinh_thanh_pho = model.TinhTP,
                phuong_xa = model.PhuongXa,
                dia_chi_cu_the = model.DiaChiCuThe,
                phuong_thuc_tt = model.PhuongThucThanhToan,
                id_ma_giam_gia = discount?.id_ma_giam_gia,
                tien_hang = cart.TongTienHang,
                tien_giam = tienGiam,
                phi_van_chuyen = cart.PhiVanChuyen,
                tong_tien = cart.TongTienHang - tienGiam + cart.PhiVanChuyen,
                trang_thai = "Chờ xác nhận",
                ghi_chu = model.GhiChu,
                thoi_gian_dat = DateTime.Now
            };

            // Tạo chi tiết hóa đơn
            var chiTiets = new List<ChiTietHoaDon>();
            foreach (var item in cart.DanhSachSanPham)
            {
                var sanPham = await _sanPhamRepository.GetByIdAsync(item.IdSanPham);

                chiTiets.Add(new ChiTietHoaDon
                {
                    id_san_pham = item.IdSanPham,
                    id_bien_the = item.IdBienThe,
                    ten_sp_luu = item.TenSanPham,
                    chat_lieu_luu = sanPham?.ChatLieu?.ten_chat_lieu,
                    mau_sac_luu = item.MauSac,
                    kich_co_luu = item.KichCo,
                    so_luong = item.SoLuong,
                    don_gia = item.DonGia
                });
            }

            var createdBill = await _donHangRepository.CreateOrderAsync(hoaDon, chiTiets);

            // Cập nhật số lần dùng discount
            if (discount != null)
            {
                discount.da_su_dung++;
                await _maGiamGiaRepository.SaveChangesAsync();
            }

            // Xóa giỏ hàng sau khi đặt hàng thành công
            await _gioHangService.XoaToanBoGioAsync();

            return createdBill;
        }

        public async Task<HoaDon> GetDonHangByIdAsync(int id)
        {
            return await _donHangRepository.GetByIdAsync(id);
        }

        public async Task<HoaDon> GetDonHangChiTietAsync(int id)
        {
            return await _donHangRepository.GetOrderWithDetailsAsync(id);
        }

        public async Task<IEnumerable<HoaDon>> GetDonHangTheoNguoiDungAsync(int userId)
        {
            return await _donHangRepository.GetOrdersByUserAsync(userId);
        }

        public async Task<bool> CapNhatTrangThaiDonHangAsync(int donHangId, string trangThai)
        {
            return await _donHangRepository.UpdateOrderStatusAsync(donHangId, trangThai);
        }
    }
}