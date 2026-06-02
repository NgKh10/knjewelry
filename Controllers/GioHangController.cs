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

        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(int sanPhamId, string tenSanPham, decimal donGia,
                                             string hinhAnh, int soLuong = 1,
                                             string size = "", string mauSac = "")
        {
            var product = await _context.SanPhams
                .Include(p => p.HinhAnhs)
                .FirstOrDefaultAsync(p => p.id_san_pham == sanPhamId);

            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });

            // Lấy giỏ hàng từ Session
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson)
                ? new List<GioHangSessionItem>()
                : JsonConvert.DeserializeObject<List<GioHangSessionItem>>(cartJson);

            // Kiểm tra sản phẩm đã tồn tại trong giỏ (cùng size và màu)
            var existing = cart.FirstOrDefault(x => x.SanPhamId == sanPhamId
                                                  && x.Size == size
                                                  && x.MauSac == mauSac);

            if (existing != null)
            {
                existing.SoLuong += soLuong;
            }
            else
            {
                cart.Add(new GioHangSessionItem
                {
                    SanPhamId = sanPhamId,
                    TenSanPham = tenSanPham ?? product.ten_sp,
                    DonGia = donGia > 0 ? donGia : (product.gia_khuyen_mai ?? product.gia),
                    HinhAnh = hinhAnh ?? product.HinhAnhs?.FirstOrDefault()?.duong_dan ?? "/images/default.jpg",
                    SoLuong = soLuong,
                    Size = size ?? "",
                    MauSac = mauSac ?? ""
                });
            }

            // Lưu lại Session
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            var cartCount = cart.Sum(x => x.SoLuong);
            HttpContext.Session.SetInt32("CartCount", cartCount);

            return Json(new { success = true, soLuongGioHang = cartCount });
        }

        public IActionResult Index()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson)
                ? new List<GioHangSessionItem>()
                : JsonConvert.DeserializeObject<List<GioHangSessionItem>>(cartJson);

            return View(cart);
        }

        [HttpPost]
        public IActionResult XoaKhoiGio(int sanPhamId)
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            var cart = string.IsNullOrEmpty(cartJson)
                ? new List<GioHangSessionItem>()
                : JsonConvert.DeserializeObject<List<GioHangSessionItem>>(cartJson);

            var item = cart.FirstOrDefault(x => x.SanPhamId == sanPhamId);
            if (item != null)
            {
                cart.Remove(item);
            }

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            var cartCount = cart.Sum(x => x.SoLuong);
            HttpContext.Session.SetInt32("CartCount", cartCount);

            return RedirectToAction("Index");
        }
    }
}