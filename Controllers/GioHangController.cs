using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using knjewelry.Models.ViewModels;
using knjewelry.Data;
using Microsoft.EntityFrameworkCore;

namespace knjewelry.Controllers
{
    public class GioHangController : Controller
    {
        private readonly TrangSucBacContext _context;

        public GioHangController(TrangSucBacContext context)
        {
            _context = context;
        }

        // ── Helper lấy/lưu giỏ hàng session ──────────────────────
        private List<GioHangSessionItem> LayGioHang()
        {
            var json = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(json)
                ? new List<GioHangSessionItem>()
                : JsonConvert.DeserializeObject<List<GioHangSessionItem>>(json)!;
        }

        private void LuuGioHang(List<GioHangSessionItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            HttpContext.Session.SetInt32("CartCount", cart.Sum(x => x.SoLuong));
        }

        // ── Thêm sản phẩm vào giỏ ────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(int sanPhamId, int? bienTheId, int soLuong = 1,
            string? kichCo = null, string? mauSac = null)
        {
            var product = await _context.SanPhams
                .Include(p => p.HinhAnhs)
                .Include(p => p.BienThes)
                .FirstOrDefaultAsync(p => p.id_san_pham == sanPhamId);

            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });

            decimal donGia  = product.gia_khuyen_mai ?? product.gia;
            string? tenKichCo = kichCo;
            string? tenMauSac = mauSac;
            int?   idBienThe  = bienTheId;

            if (bienTheId.HasValue && bienTheId.Value > 0)
            {
                var bt = product.BienThes?.FirstOrDefault(b => b.id_bien_the == bienTheId.Value);
                if (bt != null)
                {
                    donGia   += bt.gia_them;
                    tenKichCo = bt.kich_co;
                    tenMauSac = bt.mau_sac;
                    if (bt.so_luong_ton < soLuong)
                        return Json(new { success = false, message = $"Chỉ còn {bt.so_luong_ton} sản phẩm" });
                }
            }

            var cart     = LayGioHang();
            var existing = cart.FirstOrDefault(x => x.SanPhamId == sanPhamId && x.BienTheId == idBienThe);

            if (existing != null)
                existing.SoLuong += soLuong;
            else
                cart.Add(new GioHangSessionItem
                {
                    SanPhamId  = sanPhamId,
                    BienTheId  = idBienThe,
                    TenSanPham = product.ten_sp,
                    KichCo     = tenKichCo,
                    MauSac     = tenMauSac,
                    DonGia     = donGia,
                    HinhAnh    = product.HinhAnhs?.FirstOrDefault()?.duong_dan ?? "/images/default.jpg",
                    SoLuong    = soLuong
                });

            LuuGioHang(cart);
            // Trả về cartItems để JS lưu vào localStorage ngay, không cần thêm request
            return Json(new { success = true, soLuongGioHang = cart.Sum(x => x.SoLuong), cartItems = cart });
        }

        // ── Hiển thị giỏ hàng ─────────────────────────────────────
        public IActionResult Index()
        {
            return View(LayGioHang());
        }

        // ── Cập nhật số lượng ─────────────────────────────────────
        [HttpPost]
        public IActionResult CapNhatSoLuong(int sanPhamId, int? bienTheId, int soLuong)
        {
            var cart = LayGioHang();
            var item = cart.FirstOrDefault(x => x.SanPhamId == sanPhamId && x.BienTheId == bienTheId);
            if (item != null)
            {
                if (soLuong <= 0) cart.Remove(item);
                else              item.SoLuong = soLuong;
            }
            LuuGioHang(cart);
            return Json(new { success = true });
        }

        // ── Lấy toàn bộ giỏ hàng dưới dạng JSON (để localStorage lưu) ──
        [HttpGet]
        public IActionResult GetCartJson()
        {
            return Json(LayGioHang());
        }

        // ── Xóa sản phẩm khỏi giỏ ────────────────────────────────
        [HttpPost]
        public IActionResult XoaKhoiGio(int sanPhamId, int? bienTheId)
        {
            var cart = LayGioHang();
            var item = cart.FirstOrDefault(x => x.SanPhamId == sanPhamId && x.BienTheId == bienTheId);
            if (item != null) cart.Remove(item);
            LuuGioHang(cart);
            return RedirectToAction("Index");
        }

        // ── Xác nhận sản phẩm chọn để thanh toán ─────────────────
        // selectedKeys: chuỗi "sanPhamId-bienTheId,..." từ form giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanGioHang(string selectedKeys)
        {
            if (string.IsNullOrEmpty(selectedKeys))
            {
                TempData["CartError"] = "Vui lòng chọn ít nhất 1 sản phẩm để thanh toán";
                return RedirectToAction("Index");
            }

            var cart     = LayGioHang();
            var keySet   = selectedKeys.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet();

            // Lọc chỉ lấy những item được chọn
            var selected = cart.Where(x =>
            {
                var k = $"{x.SanPhamId}-{(x.BienTheId?.ToString() ?? "0")}";
                return keySet.Contains(k);
            }).ToList();

            if (!selected.Any())
            {
                TempData["CartError"] = "Không tìm thấy sản phẩm đã chọn";
                return RedirectToAction("Index");
            }

            // Lưu danh sách sản phẩm được chọn vào session riêng
            HttpContext.Session.SetString("SelectedCart", JsonConvert.SerializeObject(selected));
            return RedirectToAction("Index", "ThanhToan");
        }
    }
}
