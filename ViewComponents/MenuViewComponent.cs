using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using knjewelry.Models.Entities;

namespace knjewelry.ViewComponents
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly TrangSucBacContext _context;

        public MenuViewComponent(TrangSucBacContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh mục cha và kèm theo danh mục con
            var categories = await _context.LoaiSanPhams
                .Where(l => l.id_loai_cha == null)
                .Include(l => l.LoaiCon)  // QUAN TRỌNG: Load danh mục con
                .OrderBy(l => l.thu_tu)
                .ToListAsync();

            return View(categories);
        }
    }
}