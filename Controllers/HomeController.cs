using Microsoft.AspNetCore.Mvc;
using knjewelry.Services;
using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;

namespace knjewelry.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISanPhamService _sanPhamService;
        private readonly IGioHangService _gioHangService;

        public HomeController(ISanPhamService sanPhamService, IGioHangService gioHangService)
        {
            _sanPhamService = sanPhamService;
            _gioHangService = gioHangService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                SanPhamMoi = await _sanPhamService.GetSanPhamMoiAsync(12) ?? new List<SanPham>(),
                SanPhamHot = await _sanPhamService.GetSanPhamHotAsync(8) ?? new List<SanPham>(),
                DanhMucNoiBat = new List<LoaiSanPham>(),
                SoLuongGioHang = await _gioHangService.GetSoLuongGioHangAsync()
            };

            return View(model);
        }
    }
}