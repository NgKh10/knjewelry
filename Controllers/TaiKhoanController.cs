using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using knjewelry.Services;
using knjewelry.Models.ViewModels;
using knjewelry.Models.Entities;
using knjewelry.Data;
using Newtonsoft.Json;

namespace knjewelry.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IDonHangService _donHangService;
        private readonly TrangSucBacContext _context;

        public TaiKhoanController(ITaiKhoanService taiKhoanService, IDonHangService donHangService, TrangSucBacContext context)
        {
            _taiKhoanService = taiKhoanService;
            _donHangService = donHangService;
            _context = context;
        }

        /// <summary>
        /// Đăng nhập GET 
        /// </summary>
        [HttpGet]
        public IActionResult DangNhap(string returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// Đăng nhập POST 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            var user = await _taiKhoanService.DangNhapAsync(model.TenDangNhap, model.MatKhau);
            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Ghi session
            var hoTen = !string.IsNullOrWhiteSpace(user.ho_ten) ? user.ho_ten : user.ten_dang_nhap;
            HttpContext.Session.SetInt32("UserId", user.id_nguoi_dung);
            HttpContext.Session.SetString("UserName", hoTen);
            HttpContext.Session.SetString("UserRole", user.vai_tro ?? "khach_hang");

            // Cookie "remember me"
            Response.Cookies.Append("KN_Remember", user.id_nguoi_dung.ToString(), new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            });

            // Khôi phục giỏ hàng đã lưu của tài khoản
            try
            {
                var gioHangDaLuu = await LayGioHangTuTaiKhoanAsync(user.id_nguoi_dung);
                var gioHangKhach = LayGioHangSession();
                var gioHangGop = GopGioHang(gioHangKhach, gioHangDaLuu);
                LuuGioHangSession(gioHangGop);
            }
            catch { }

            // Nếu có returnUrl, chuyển hướng về đó
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user.vai_tro == "quan_tri")
                return Redirect("/admin/");

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Đăng ký GET 
        /// </summary>
        [HttpGet]
        public IActionResult DangKy() => View();

        /// <summary>
        /// Đăng ký POST 
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                await _taiKhoanService.DangKyAsync(model);
                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(DangNhap));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        /// <summary>
        /// Thông tin tài khoản GET 
        /// </summary>
        public async Task<IActionResult> ThongTin()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(DangNhap));

            var user = await _taiKhoanService.GetThongTinAsync(userId.Value);
            if (user == null) return NotFound();

            return View(new CapNhatThongTinViewModel
            {
                HoTen = user.ho_ten ?? "",
                Email = user.email ?? "",
                SoDienThoai = user.so_dien_thoai ?? "",
                DiaChi = user.dia_chi ?? ""
            });
        }

        /// <summary>
        /// Thông tin tài khoản POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTin(CapNhatThongTinViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(DangNhap));

            if (await _taiKhoanService.CapNhatThongTinAsync(userId.Value, model))
            {
                TempData["Success"] = "Cập nhật thông tin thành công";
                HttpContext.Session.SetString("UserName", model.HoTen ?? "");
            }
            return View(model);
        }

        /// <summary>
        /// Đơn hàng 
        /// </summary>
        public async Task<IActionResult> DonHang()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(DangNhap));
            return View(await _donHangService.GetDonHangTheoNguoiDungAsync(userId.Value));
        }

        /// <summary>
        /// Chi tiết đơn hàng 
        /// </summary>
        public async Task<IActionResult> ChiTietDonHang(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(DangNhap));
            var order = await _donHangService.GetDonHangChiTietAsync(id);
            return order == null ? NotFound() : View(order);
        }

        /// <summary>
        /// Đổi mật khẩu GET
        /// </summary>
        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction(nameof(DangNhap));
            return View();
        }

        /// <summary>
        /// Đổi mật khẩu POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction(nameof(DangNhap));

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.NguoiDungs.FindAsync(userId.Value);
            if (user == null)
                return RedirectToAction(nameof(DangNhap));

            // Kiểm tra mật khẩu cũ
            if (user.mat_khau != model.MatKhauHienTai)
            {
                ModelState.AddModelError("MatKhauHienTai", "Mật khẩu hiện tại không đúng");
                return View(model);
            }

            // Cập nhật mật khẩu mới
            user.mat_khau = model.MatKhauMoi;
            _context.NguoiDungs.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("ThongTin");
        }

        /// <summary>
        /// Đăng xuất 
        /// </summary>
        public async Task<IActionResult> DangXuat()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            // Lưu giỏ hàng hiện tại vào tài khoản TRƯỚC khi xóa session
            if (userId.HasValue)
            {
                var cart = LayGioHangSession();
                if (cart.Any())
                    await LuuGioHangVaoTaiKhoanAsync(userId.Value, cart);
            }

            // Xóa cookie remember để không tự đăng nhập lại
            // (phải truyền đúng Path đã dùng lúc tạo cookie, nếu không trình
            // duyệt sẽ không xóa được — mặc định Path suy ra từ URL hiện tại)
            Response.Cookies.Delete("KN_Remember", new CookieOptions { Path = "/" });

            // Báo cho trình duyệt xóa luôn bản sao giỏ hàng trong localStorage
            // (nếu không, cơ chế đồng bộ AppContext sẽ "phục hồi" giỏ hàng cũ
            // ngay sau khi đăng xuất, khiến giỏ hàng không thật sự về 0)
            // Path = "/" bắt buộc phải có — nếu không, mặc định cookie sẽ bị
            // giới hạn trong "/TaiKhoan" (thư mục của action DangXuat) và sẽ
            // KHÔNG được gửi kèm khi trình duyệt redirect sang "/Home/Index",
            // khiến AppStateViewComponent không bao giờ nhận được tín hiệu này.
            Response.Cookies.Append("KN_ClearCart", "1", new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddMinutes(2),
                IsEssential = true
            });

            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ==================== HELPERS ====================

        private List<GioHangSessionItem> LayGioHangSession()
        {
            var json = HttpContext.Session.GetString("Cart");
            return string.IsNullOrEmpty(json)
                ? new List<GioHangSessionItem>()
                : JsonConvert.DeserializeObject<List<GioHangSessionItem>>(json) ?? new List<GioHangSessionItem>();
        }

        private void LuuGioHangSession(List<GioHangSessionItem> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            HttpContext.Session.SetInt32("CartCount", cart.Sum(x => x.SoLuong));
        }

        private List<GioHangSessionItem> GopGioHang(List<GioHangSessionItem> gioHangChinh, List<GioHangSessionItem> gioHangPhu)
        {
            var ketQua = new List<GioHangSessionItem>(gioHangChinh);
            foreach (var item in gioHangPhu)
            {
                var existing = ketQua.FirstOrDefault(x => x.SanPhamId == item.SanPhamId && x.BienTheId == item.BienTheId);
                if (existing != null) existing.SoLuong += item.SoLuong;
                else ketQua.Add(item);
            }
            return ketQua;
        }

        private async Task LuuGioHangVaoTaiKhoanAsync(int userId, List<GioHangSessionItem> items)
        {
            var cart = await _context.GioHangs.FirstOrDefaultAsync(g => g.id_nguoi_dung == userId);
            if (cart == null)
            {
                cart = new GioHang
                {
                    ma_phien = $"taikhoan-{userId}",
                    id_nguoi_dung = userId,
                    ngay_tao = DateTime.Now,
                    ngay_cap_nhat = DateTime.Now
                };
                _context.GioHangs.Add(cart);
                await _context.SaveChangesAsync();
            }

            var chiTietCu = await _context.ChiTietGioHangs.Where(c => c.id_gio_hang == cart.id_gio_hang).ToListAsync();
            _context.ChiTietGioHangs.RemoveRange(chiTietCu);

            foreach (var item in items)
            {
                _context.ChiTietGioHangs.Add(new ChiTietGioHang
                {
                    id_gio_hang = cart.id_gio_hang,
                    id_san_pham = item.SanPhamId,
                    id_bien_the = item.BienTheId,
                    so_luong = item.SoLuong,
                    don_gia = item.DonGia,
                    ngay_tao = DateTime.Now
                });
            }

            cart.ngay_cap_nhat = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        private async Task<List<GioHangSessionItem>> LayGioHangTuTaiKhoanAsync(int userId)
        {
            var cart = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs).ThenInclude(c => c.SanPham).ThenInclude(p => p.HinhAnhs)
                .Include(g => g.ChiTietGioHangs).ThenInclude(c => c.BienThe)
                .FirstOrDefaultAsync(g => g.id_nguoi_dung == userId);

            if (cart?.ChiTietGioHangs == null || !cart.ChiTietGioHangs.Any())
                return new List<GioHangSessionItem>();

            return cart.ChiTietGioHangs.Select(item => new GioHangSessionItem
            {
                SanPhamId = item.id_san_pham,
                BienTheId = item.id_bien_the,
                TenSanPham = item.SanPham?.ten_sp ?? "",
                KichCo = item.BienThe?.kich_co,
                MauSac = item.BienThe?.mau_sac,
                DonGia = item.don_gia,
                HinhAnh = item.SanPham?.HinhAnhs?.FirstOrDefault()?.duong_dan ?? "/images/default.jpg",
                SoLuong = item.so_luong
            }).ToList();
        }
    }
}