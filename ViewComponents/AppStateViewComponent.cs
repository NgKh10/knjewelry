using Microsoft.AspNetCore.Mvc;
using knjewelry.Data;

namespace knjewelry.ViewComponents
{
    /// <summary>
    /// ViewComponent quản lý 2 trạng thái bền vững:
    /// 1. Tài khoản: restore session từ cookie 7 ngày (không cần đăng nhập lại)
    /// 2. Giỏ hàng: khởi tạo JavaScript context để sync với localStorage
    /// </summary>
    public class AppStateViewComponent : ViewComponent
    {
        private readonly TrangSucBacContext _db;

        public AppStateViewComponent(TrangSucBacContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // ── Trạng thái 1: Tài khoản ──────────────────────────────────
            // Nếu session trống nhưng có cookie "remember" → tự restore session
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
            {
                var rememberCookie = HttpContext.Request.Cookies["KN_Remember"];
                if (!string.IsNullOrEmpty(rememberCookie) &&
                    int.TryParse(rememberCookie, out var rememberedId))
                {
                    var user = await _db.NguoiDungs.FindAsync(rememberedId);
                    if (user != null && user.trang_thai == 1)
                    {
                        var hoTen = !string.IsNullOrWhiteSpace(user.ho_ten)
                            ? user.ho_ten : user.ten_dang_nhap;

                        HttpContext.Session.SetInt32("UserId", user.id_nguoi_dung);
                        HttpContext.Session.SetString("UserName", hoTen);
                        HttpContext.Session.SetString("UserRole", user.vai_tro ?? "khach_hang");
                    }
                }
            }

            // ── Trạng thái 2: Giỏ hàng ──────────────────────────────────
            // Dữ liệu để JavaScript biết session cart hiện tại
            var sessionCartCount = HttpContext.Session.GetInt32("CartCount") ?? 0;
            var isLoggedIn = HttpContext.Session.GetInt32("UserId").HasValue;

            // Sau khi đăng xuất, server đặt cookie "KN_ClearCart" để báo JS
            // xóa luôn bản sao giỏ hàng trong localStorage — tránh việc
            // AppContext "phục hồi" lại giỏ hàng cũ ngay sau khi đăng xuất
            var clearLocalCart = false;
            if (HttpContext.Request.Cookies.ContainsKey("KN_ClearCart"))
            {
                clearLocalCart = true;
                HttpContext.Response.Cookies.Delete("KN_ClearCart");
            }

            ViewData["SessionCartCount"] = sessionCartCount;
            ViewData["IsLoggedIn"] = isLoggedIn;
            ViewData["ClearLocalCart"] = clearLocalCart;

            return View();
        }
    }
}
