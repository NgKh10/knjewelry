using Microsoft.AspNetCore.Mvc;
using knjewelry.Services;

namespace knjewelry.ViewComponents
{
    public class SanPhamBanChay : ViewComponent
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamBanChay(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int soLuong = 8)
        {
            var products = await _sanPhamService.GetSanPhamHotAsync(soLuong);
            return View(products);
        }
    }
}