using Microsoft.AspNetCore.Mvc;
using knjewelry.Repository;

namespace knjewelry.ViewComponents
{
    public class DanhMucMenuViewComponent : ViewComponent
    {
        private readonly IGenericRepository<Models.Entities.LoaiSanPham> _loaiRepository;

        public DanhMucMenuViewComponent(IGenericRepository<Models.Entities.LoaiSanPham> loaiRepository)
        {
            _loaiRepository = loaiRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _loaiRepository.FindAsync(l => l.id_loai_cha == null && l.ten_loai != null); return View(categories.OrderBy(c => c.thu_tu));
        }
    }
}