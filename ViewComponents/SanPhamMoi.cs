using Microsoft.AspNetCore.Mvc;
using knjewelry.Services;

namespace knjewelry.ViewComponents
{
    public class SanPhamMoiViewComponent : ViewComponent
    {
        private readonly ISanPhamService _sanPhamService;

        public SanPhamMoiViewComponent(ISanPhamService sanPhamService)
        {
            _sanPhamService = sanPhamService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int soLuong = 12)
        {
            var products = await _sanPhamService.GetSanPhamMoiAsync(soLuong);
            return View(products);
        }
    }
}