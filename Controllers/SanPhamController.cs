using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;

namespace knjewelry.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly TrangSucBacContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SanPhamController(TrangSucBacContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Lấy số lượng giỏ hàng từ session
        private async Task<int> GetCartCount()
        {
            // Lấy từ session hoặc database
            var cartCount = _httpContextAccessor.HttpContext.Session.GetInt32("CartCount");
            if (cartCount == null)
            {
                // Nếu chưa có, tính từ database (nếu đã đăng nhập)
                var userId = _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    var cart = await _context.GioHangs
                        .Include(g => g.ChiTietGioHangs)
                        .FirstOrDefaultAsync(g => g.id_nguoi_dung == userId.Value);
                    cartCount = cart?.ChiTietGioHangs?.Sum(c => c.so_luong) ?? 0;
                    _httpContextAccessor.HttpContext.Session.SetInt32("CartCount", cartCount.Value);
                }
                else
                {
                    cartCount = 0;
                }
            }
            return cartCount.Value;
        }

        public async Task<IActionResult> Index(int? loaiId, string search, int page = 1,
                                        int? giaMin = null, int? giaMax = null,
                                        string sort = null)
        {
            int pageSize = 12;
            var query = _context.SanPhams
                .Include(p => p.LoaiSanPham)
                .Include(p => p.ChatLieu)
                .Include(p => p.HinhAnhs.Where(i => i.la_chinh))
                .Where(p => p.trang_thai == 1);

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(p => p.ten_sp.Contains(search));
            }

            // Lọc theo loại (bao gồm cả loại con)
            if (loaiId.HasValue)
            {
                var loaiConIds = await _context.LoaiSanPhams
                    .Where(l => l.id_loai_cha == loaiId.Value)
                    .Select(l => l.id_loai_sp)
                    .ToListAsync();

                if (loaiConIds.Any())
                {
                    query = query.Where(p => loaiConIds.Contains(p.id_loai_sp));
                }
                else
                {
                    query = query.Where(p => p.id_loai_sp == loaiId.Value);
                }
            }

            // Lọc theo giá
            if (giaMin.HasValue && giaMin.Value > 0)
            {
                query = query.Where(p => (p.gia_khuyen_mai ?? p.gia) >= giaMin.Value);
            }
            if (giaMax.HasValue && giaMax.Value > 0)
            {
                query = query.Where(p => (p.gia_khuyen_mai ?? p.gia) <= giaMax.Value);
            }

            // SẮP XẾP THEO
            System.Diagnostics.Debug.WriteLine($"Sort value: {sort}");

            switch (sort)
            {
                case "price-asc":
                    query = query.OrderBy(p => p.gia_khuyen_mai ?? p.gia);
                    break;
                case "price-desc":
                    query = query.OrderByDescending(p => p.gia_khuyen_mai ?? p.gia);
                    break;
                case "name-asc":
                    query = query.OrderBy(p => p.ten_sp);
                    break;
                case "name-desc":
                    query = query.OrderByDescending(p => p.ten_sp);
                    break;
                default:
                    query = query.OrderByDescending(p => p.ngay_tao);
                    break;
            }


            int totalItems = await query.CountAsync();
            //PHÂN TRANG 
            var products = await query
                .OrderByDescending(p => p.ngay_tao)
                .Skip((page - 1) * pageSize)  // bỏ qua page của trang trước 
                .Take(pageSize) // lấy 12 bản ghi tiếp theo
                .ToListAsync(); // gọi database

            var viewModel = new DanhSachSanPhamViewModel
            {
                DanhSachSanPham = products,
                TrangHienTai = page,
                TongTrang = (int)Math.Ceiling(totalItems / (double)pageSize),
                LoaiId = loaiId,
                TuKhoa = search
            };

            //truyền giá trị tìm kiếm sang view 
            ViewBag.Sort = sort;
            ViewBag.GiaMin = giaMin;
            ViewBag.GiaMax = giaMax;
            ViewBag.LoaiId = loaiId;
            ViewBag.TuKhoa = search;
            ViewBag.CartCount = await GetCartCount();

            return View(viewModel);
        }
 

        public async Task<IActionResult> Detail(int id)
        {
            var product = await _context.SanPhams
                .Include(p => p.LoaiSanPham)
                .Include(p => p.ChatLieu)
                .Include(p => p.HinhAnhs.OrderBy(i => i.thu_tu))
                .Include(p => p.BienThes)
                .FirstOrDefaultAsync(p => p.id_san_pham == id && p.trang_thai == 1);

            if (product == null) return NotFound();

            // Lấy sản phẩm liên quan (cùng danh mục)
            var relatedProducts = await _context.SanPhams
                .Include(p => p.HinhAnhs.Where(h => h.la_chinh))
                .Where(p => p.id_loai_sp == product.id_loai_sp && p.id_san_pham != id && p.trang_thai == 1)
                .Take(6)
                .ToListAsync();

            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.CartCount = await GetCartCount();

            return View(product);
        }
    }
}