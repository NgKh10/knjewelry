using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using knjewelry.Models.ViewModels;
using knjewelry.Data;
using knjewelry.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace knjewelry.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly TrangSucBacContext _context;

        public ThanhToanController(TrangSucBacContext context)
        {
            _context = context;
        }

        // ── Lấy giỏ hàng đã được chọn (từ XacNhanGioHang) ────────
        private List<GioHangSessionItem> LaySelectedCart()
        {
            var json = HttpContext.Session.GetString("SelectedCart");
            if (!string.IsNullOrEmpty(json))
                return JsonConvert.DeserializeObject<List<GioHangSessionItem>>(json)!;

            // Fallback: nếu không có SelectedCart thì lấy toàn bộ giỏ hàng
            var cartJson = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(cartJson)
                ? new List<GioHangSessionItem>()
                : JsonConvert.DeserializeObject<List<GioHangSessionItem>>(cartJson)!;
        }

        private GioHangViewModel BuildGioHangVM(List<GioHangSessionItem> items)
        {
            var vm = new GioHangViewModel
            {
                DanhSachSanPham = items.Select(item => new GioHangItemViewModel
                {
                    IdSanPham    = item.SanPhamId,
                    TenSanPham   = item.TenSanPham,
                    DonGia       = item.DonGia,
                    SoLuong      = item.SoLuong,
                    DuongDanAnh  = item.HinhAnh,
                    ThanhTien    = item.ThanhTien,
                    KichCo       = item.KichCo,
                    MauSac       = item.MauSac
                }).ToList()
            };
            vm.TinhTong();
            return vm;
        }

        // ── GET: Trang thanh toán ──────────────────────────────────
        [HttpGet]
        public IActionResult Index()
        {
            var cartItems = LaySelectedCart();
            if (!cartItems.Any())
                return RedirectToAction("Index", "GioHang");

            var model = new ThanhToanViewModel
            {
                GioHang = BuildGioHangVM(cartItems)
            };

            // Điền trước thông tin nếu đã đăng nhập
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var user = _context.NguoiDungs.Find(userId.Value);
                if (user != null)
                {
                    model.HoTen        = user.ho_ten;
                    model.Email        = user.email;
                    model.SoDienThoai  = user.so_dien_thoai;
                }
            }

            return View(model);
        }

        // ── POST: Đặt hàng ────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> HoanTat(ThanhToanViewModel model)
        {
            // Luôn reload GioHang để tránh NullReference khi cần re-render form
            var cartItems = LaySelectedCart();
            model.GioHang = BuildGioHangVM(cartItems);

            if (!cartItems.Any())
                return RedirectToAction("Index", "GioHang");

            if (!ModelState.IsValid)
                return View("Index", model);

            try
            {
                var userId       = HttpContext.Session.GetInt32("UserId");
                var tienHang     = cartItems.Sum(x => x.ThanhTien);
                var phiVanChuyen = tienHang >= 150000 ? 0 : 30000;

                // Áp dụng mã giảm giá (nếu có)
                decimal tienGiam    = 0;
                int?    idMaGiamGia = null;

                if (!string.IsNullOrEmpty(model.MaGiamGia))
                {
                    var ma = await _context.MaGiamGias.FirstOrDefaultAsync(
                        m => m.ma_code == model.MaGiamGia.ToUpper() && m.trang_thai == 1);

                    if (ma != null && tienHang >= ma.don_hang_toi_thieu)
                    {
                        tienGiam    = TinhSoGiam(ma, tienHang);
                        idMaGiamGia = ma.id_ma_giam_gia;
                        ma.da_su_dung++;  // Tăng lượt đã dùng
                    }
                }

                var tongTien = tienHang - tienGiam + phiVanChuyen;

                // Gộp PhuongXa + QuanHuyen (entity không có cột riêng cho QuanHuyen)
                var phuongXaDayDu = string.IsNullOrEmpty(model.QuanHuyen)
                    ? model.PhuongXa
                    : $"{model.PhuongXa}, {model.QuanHuyen}";

                var hoaDon = new HoaDon
                {
                    id_nguoi_dung  = userId,
                    ho_ten         = model.HoTen,
                    email          = model.Email,
                    so_dien_thoai  = model.SoDienThoai,
                    tinh_thanh_pho = model.TinhTP,
                    phuong_xa      = phuongXaDayDu,
                    dia_chi_cu_the = model.DiaChiCuThe,
                    phuong_thuc_tt = model.PhuongThucThanhToan,
                    id_ma_giam_gia = idMaGiamGia,
                    tien_hang      = tienHang,
                    tien_giam      = tienGiam,
                    phi_van_chuyen = phiVanChuyen,
                    tong_tien      = tongTien,
                    trang_thai     = "Chờ xác nhận",
                    ghi_chu        = model.GhiChu ?? string.Empty,
                    thoi_gian_dat  = DateTime.Now
                };

                _context.HoaDons.Add(hoaDon);
                await _context.SaveChangesAsync();

                // Lưu chi tiết hóa đơn + trừ tồn kho biến thể theo số lượng đã mua
                foreach (var item in cartItems)
                {
                    _context.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        id_hoa_don    = hoaDon.id_hoa_don,
                        id_san_pham   = item.SanPhamId,
                        id_bien_the   = item.BienTheId,
                        ten_sp_luu    = item.TenSanPham,
                        kich_co_luu   = item.KichCo   ?? string.Empty,
                        mau_sac_luu   = item.MauSac   ?? string.Empty,
                        chat_lieu_luu = string.Empty,
                        so_luong      = item.SoLuong,
                        don_gia       = item.DonGia
                    });

                    if (item.BienTheId.HasValue)
                    {
                        var bienThe = await _context.BienThes.FindAsync(item.BienTheId.Value);
                        if (bienThe != null)
                            bienThe.so_luong_ton = Math.Max(0, bienThe.so_luong_ton - item.SoLuong);
                    }
                }
                await _context.SaveChangesAsync();

                // Xóa SelectedCart và các sản phẩm đã đặt khỏi giỏ hàng chính
                HttpContext.Session.Remove("SelectedCart");
                XoaSelectedKhoiGioHangChinh(cartItems);

                return RedirectToAction("ThanhCong", new { id = hoaDon.id_hoa_don });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đặt hàng thất bại: " + ex.Message);
                return View("Index", model);
            }
        }

        // ── AJAX kiểm tra mã giảm giá ─────────────────────────────
        [HttpPost]
        public async Task<IActionResult> KiemTraMaGiamGia(string code, decimal total)
        {
            if (string.IsNullOrEmpty(code))
                return Json(new { success = false, message = "Vui lòng nhập mã giảm giá" });

            var ma = await _context.MaGiamGias.FirstOrDefaultAsync(
                m => m.ma_code == code.ToUpper() && m.trang_thai == 1);

            if (ma == null)
                return Json(new { success = false, message = "Mã giảm giá không tồn tại hoặc đã ngừng hoạt động" });

            if (ma.ngay_bat_dau.HasValue && DateTime.Now < ma.ngay_bat_dau.Value)
                return Json(new { success = false, message = "Mã giảm giá chưa có hiệu lực" });

            if (ma.ngay_ket_thuc.HasValue && DateTime.Now > ma.ngay_ket_thuc.Value)
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn" });

            if (total < ma.don_hang_toi_thieu)
                return Json(new { success = false, message = $"Đơn hàng tối thiểu {ma.don_hang_toi_thieu:N0}₫ để dùng mã này" });

            if (ma.so_luong.HasValue && ma.da_su_dung >= ma.so_luong.Value)
                return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng" });

            var soGiam = TinhSoGiam(ma, total);
            var moTa   = ma.loai_giam is "phan_tram" or "%"
                ? $"Giảm {ma.gia_tri}%{(ma.giam_toi_da.HasValue ? $" (tối đa {ma.giam_toi_da.Value:N0}₫)" : "")}"
                : $"Giảm {ma.gia_tri:N0}₫";

            return Json(new { success = true, soGiam, moTa, code = ma.ma_code });
        }

        // ── Helper tính số tiền giảm ──────────────────────────────
        private static decimal TinhSoGiam(MaGiamGia ma, decimal tienHang)
        {
            decimal giam = ma.loai_giam is "phan_tram" or "%"
                ? tienHang * ma.gia_tri / 100
                : ma.gia_tri;

            if (ma.loai_giam is "phan_tram" or "%" && ma.giam_toi_da.HasValue)
                giam = Math.Min(giam, ma.giam_toi_da.Value);

            return Math.Min(giam, tienHang); // không giảm quá tổng tiền hàng
        }

        // ── Xóa các SP đã đặt khỏi giỏ hàng chính ───────────────
        private void XoaSelectedKhoiGioHangChinh(List<GioHangSessionItem> ordered)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson)) return;

            var cart = JsonConvert.DeserializeObject<List<GioHangSessionItem>>(cartJson)!;
            foreach (var o in ordered)
            {
                var match = cart.FirstOrDefault(x => x.SanPhamId == o.SanPhamId && x.BienTheId == o.BienTheId);
                if (match != null) cart.Remove(match);
            }
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            HttpContext.Session.SetInt32("CartCount", cart.Sum(x => x.SoLuong));
        }

        // ── Trang thành công ──────────────────────────────────────
        public async Task<IActionResult> ThanhCong(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefaultAsync(h => h.id_hoa_don == id);

            if (hoaDon == null) return NotFound();
            return View(hoaDon);
        }
    }
}
