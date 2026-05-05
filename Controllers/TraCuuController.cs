using Microsoft.AspNetCore.Mvc;
using knjewelry.Services;
using knjewelry.Models.ViewModels;

namespace knjewelry.Controllers
{
    public class TraCuuController : Controller
    {
        private readonly IDonHangService _donHangService;

        public TraCuuController(IDonHangService donHangService)
        {
            _donHangService = donHangService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(TraCuuDonHangViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var orders = await _donHangService.GetDonHangTheoNguoiDungAsync(0);
            var order = orders.FirstOrDefault(o => o.ma_hoa_don == model.MaDonHang && o.email == model.Email);

            if (order == null)
            {
                ModelState.AddModelError("", "Không tìm thấy đơn hàng");
                return View(model);
            }

            var detail = await _donHangService.GetDonHangChiTietAsync(order.id_hoa_don);
            return View("KetQua", detail);
        }
    }
}