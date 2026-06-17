using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using knjewelry.Models.Entities;
using Newtonsoft.Json;

namespace knjewelry.Controllers
{
    public class YeuThichController : Controller
    {
        private readonly TrangSucBacContext _context;

        public YeuThichController(TrangSucBacContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            List<int> wishlistIds = new List<int>();

            if (userId.HasValue)
            {
                // Đã đăng nhập -> lấy từ database
                wishlistIds = await _context.YeuThichs   
                    .Where(w => w.id_nguoi_dung == userId.Value)
                    .Select(w => w.id_san_pham)
                    .ToListAsync();

                // Đồng bộ localStorage (gửi về client để cập nhật)
                ViewBag.WishlistJson = JsonConvert.SerializeObject(wishlistIds);
            }
            else
            {
                // Chưa đăng nhập -> lấy từ Session
                var wishlistJson = HttpContext.Session.GetString("Wishlist");
                if (!string.IsNullOrEmpty(wishlistJson))
                {
                    wishlistIds = JsonConvert.DeserializeObject<List<int>>(wishlistJson) ?? new List<int>();
                }
                ViewBag.WishlistJson = JsonConvert.SerializeObject(wishlistIds);
            }

            if (!wishlistIds.Any())
            {
                return View(new List<SanPham>());
            }

            var products = await _context.SanPhams
                .Include(p => p.HinhAnhs.Where(i => i.la_chinh))
                .Include(p => p.LoaiSanPham)
                .Include(p => p.ChatLieu)
                .Where(p => wishlistIds.Contains(p.id_san_pham) && p.trang_thai == 1)
                .ToListAsync();

            ViewBag.WishlistIds = wishlistIds;
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> Them(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                var existing = await _context.YeuThichs
                    .FirstOrDefaultAsync(w => w.id_nguoi_dung == userId.Value && w.id_san_pham == id);

                if (existing == null)
                {
                    _context.YeuThichs.Add(new YeuThich
                    {
                        id_nguoi_dung = userId.Value,
                        id_san_pham = id,
                        ngay_tao = DateTime.Now
                    });
                    await _context.SaveChangesAsync();
                }
                return Json(new { success = true, message = "Đã thêm vào yêu thích" });
            }
            else
            {
                var wishlistJson = HttpContext.Session.GetString("Wishlist");
                var wishlist = string.IsNullOrEmpty(wishlistJson)
                    ? new List<int>()
                    : JsonConvert.DeserializeObject<List<int>>(wishlistJson);

                if (!wishlist.Contains(id))
                {
                    wishlist.Add(id);
                    HttpContext.Session.SetString("Wishlist", JsonConvert.SerializeObject(wishlist));
                }
                return Json(new { success = true, message = "Đã thêm vào yêu thích" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Xoa(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                var item = await _context.YeuThichs
                    .FirstOrDefaultAsync(w => w.id_nguoi_dung == userId.Value && w.id_san_pham == id);

                if (item != null)
                {
                    _context.YeuThichs.Remove(item);
                    await _context.SaveChangesAsync();
                }
                return Json(new { success = true, message = "Đã xóa khỏi yêu thích" });
            }
            else
            {
                var wishlistJson = HttpContext.Session.GetString("Wishlist");
                if (!string.IsNullOrEmpty(wishlistJson))
                {
                    var wishlist = JsonConvert.DeserializeObject<List<int>>(wishlistJson);
                    if (wishlist != null && wishlist.Contains(id))
                    {
                        wishlist.Remove(id);
                        HttpContext.Session.SetString("Wishlist", JsonConvert.SerializeObject(wishlist));
                    }
                }
                return Json(new { success = true, message = "Đã xóa khỏi yêu thích" });
            }
        }
    }
}