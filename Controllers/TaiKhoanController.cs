using Microsoft.AspNetCore.Mvc;
using knjewelry.Services;
using knjewelry.Models.ViewModels;

namespace knjewelry.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ITaiKhoanService _taiKhoanService;
        private readonly IGioHangService _gioHangService;
        private readonly IDonHangService _donHangService;

        public TaiKhoanController(
            ITaiKhoanService taiKhoanService,
            IGioHangService gioHangService,
            IDonHangService donHangService)
        {
            _taiKhoanService = taiKhoanService;
            _gioHangService = gioHangService;
            _donHangService = donHangService;
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _taiKhoanService.DangNhapAsync(model.TenDangNhap, model.MatKhau);
            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            // Hợp nhất giỏ hàng từ session vào user
            await _gioHangService.HopNhatGioHangSauDangNhapAsync(user.id_nguoi_dung);

            HttpContext.Session.SetInt32("UserId", user.id_nguoi_dung);
            HttpContext.Session.SetString("UserName", user.ho_ten);
            HttpContext.Session.SetString("UserRole", user.vai_tro);

            if (user.vai_tro == "quan_tri")
                return RedirectToAction("Index", "Admin");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

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

        public async Task<IActionResult> ThongTin()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(DangNhap));

            var user = await _taiKhoanService.GetThongTinAsync(userId.Value);
            if (user == null) return NotFound();

            var model = new CapNhatThongTinViewModel
            {
                HoTen = user.ho_ten,
                Email = user.email,
                SoDienThoai = user.so_dien_thoai,
                DiaChi = user.dia_chi
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTin(CapNhatThongTinViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(DangNhap));

            var result = await _taiKhoanService.CapNhatThongTinAsync(userId.Value, model);
            if (result)
            {
                TempData["Success"] = "Cập nhật thông tin thành công";
                var user = await _taiKhoanService.GetThongTinAsync(userId.Value);
                HttpContext.Session.SetString("UserName", user.ho_ten);
            }
            return View(model);
        }

        public async Task<IActionResult> DonHang()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(DangNhap));

            var orders = await _donHangService.GetDonHangTheoNguoiDungAsync(userId.Value);
            return View(orders);
        }

        public async Task<IActionResult> ChiTietDonHang(int id) 
        {
            var order = await _donHangService.GetDonHangChiTietAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpGet]
        public IActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       /* public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction(nameof(DangNhap));

            var result = await _taiKhoanService.DoiMatKhauAsync(userId.Value, model.MatKhauCu, model.MatKhauMoi);
            if (!result)
            {
                ModelState.AddModelError("", "Mật khẩu cũ không đúng");
                return View(model);
            }

            TempData["Success"] = "Đổi mật khẩu thành công";
            return RedirectToAction(nameof(ThongTin));
        }*/

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}