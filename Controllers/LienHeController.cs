using Microsoft.AspNetCore.Mvc;

namespace knjewelry.Controllers
{
    public class LienHeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GuiLienHe(string hoTen, string email, string soDienThoai, string tieuDe, string noiDung)
        {
            // Xử lý gửi liên hệ (lưu vào database hoặc gửi email)
            TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất.";
            return RedirectToAction(nameof(Index));
        }
    }
}