using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using knjewelry.Models.ViewModels;
using knjewelry.Data;
using knjewelry.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace knjewelry.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly TrangSucBacContext _context;

        public ThanhToanController(TrangSucBacContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return RedirectToAction("Index", "GioHang");
            }

            var cart = JsonConvert.DeserializeObject<List<GioHangSessionItem>>(cartJson);
            if (!cart.Any())
            {
                return RedirectToAction("Index", "GioHang");
            }

            var gioHangViewModel = new GioHangViewModel
            {
                DanhSachSanPham = cart.Select(item => new GioHangItemViewModel
                {
                    IdSanPham = item.SanPhamId,
                    TenSanPham = item.TenSanPham,
                    DonGia = item.DonGia,
                    SoLuong = item.SoLuong,
                    DuongDanAnh = item.HinhAnh,
                    ThanhTien = item.ThanhTien
                }).ToList()
            };
            gioHangViewModel.TinhTong();

            var model = new ThanhToanViewModel
            {
                GioHang = gioHangViewModel
            };

            // Nếu đã đăng nhập, lấy thông tin user
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var user = _context.NguoiDungs.Find(userId.Value);
                if (user != null)
                {
                    model.HoTen = user.ho_ten;
                    model.Email = user.email;
                    model.SoDienThoai = user.so_dien_thoai;
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> HoanTat(ThanhToanViewModel model)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    // Load lại giỏ hàng
                    var cartJson = HttpContext.Session.GetString("Cart");
                    if (!string.IsNullOrEmpty(cartJson))
                    {
                        var cart = JsonConvert.DeserializeObject<List<GioHangSessionItem>>(cartJson);
                        var gioHangViewModel = new GioHangViewModel
                        {
                            DanhSachSanPham = cart.Select(item => new GioHangItemViewModel
                            {
                                IdSanPham = item.SanPhamId,
                                TenSanPham = item.TenSanPham,
                                DonGia = item.DonGia,
                                SoLuong = item.SoLuong,
                                DuongDanAnh = item.HinhAnh,
                                ThanhTien = item.ThanhTien
                            }).ToList()
                        };
                        gioHangViewModel.TinhTong();
                        model.GioHang = gioHangViewModel;
                    }
                    return View("Index", model);
                }

                // Lấy giỏ hàng từ Session
                var cartJson2 = HttpContext.Session.GetString("Cart");
                if (string.IsNullOrEmpty(cartJson2))
                {
                    return RedirectToAction("Index", "GioHang");
                }

                var cartItems = JsonConvert.DeserializeObject<List<GioHangSessionItem>>(cartJson2);
                if (!cartItems.Any())
                {
                    return RedirectToAction("Index", "GioHang");
                }

                var userId = HttpContext.Session.GetInt32("UserId");
                var tienHang = cartItems.Sum(x => x.ThanhTien);
                var phiVanChuyen = tienHang >= 150000 ? 0 : 30000;
                var tongTien = tienHang + phiVanChuyen;

                // Tạo hóa đơn
                var hoaDon = new HoaDon
                {
                    id_nguoi_dung = userId,
                    ho_ten = model.HoTen,
                    email = model.Email,
                    so_dien_thoai = model.SoDienThoai,
                    tinh_thanh_pho = model.TinhTP,
                    phuong_xa = model.PhuongXa,
                    dia_chi_cu_the = model.DiaChiCuThe,
                    phuong_thuc_tt = model.PhuongThucThanhToan,
                    tien_hang = tienHang,
                    phi_van_chuyen = phiVanChuyen,
                    tong_tien = tongTien,
                    trang_thai = "Chờ xác nhận",
                    ghi_chu = model.GhiChu,
                    thoi_gian_dat = DateTime.Now
                };
                //insert into HoaDon 
                _context.HoaDons.Add(hoaDon);
                await _context.SaveChangesAsync();

                // Tạo chi tiết hóa đơn
                foreach (var item in cartItems)
                {
                    var chiTiet = new ChiTietHoaDon
                    {
                        id_hoa_don = hoaDon.id_hoa_don,
                        id_san_pham = item.SanPhamId,
                        ten_sp_luu = item.TenSanPham,
                        so_luong = item.SoLuong,
                        don_gia = item.DonGia
                    };
                    _context.ChiTietHoaDons.Add(chiTiet); // thêm cthd 
                }
                //lưu thay đổi 
                await _context.SaveChangesAsync();

                // Xóa giỏ hàng sau khi đặt hàng thành công
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.SetInt32("CartCount", 0);

                return RedirectToAction("ThanhCong", new { id = hoaDon.id_hoa_don });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LỖI: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"STACK: {ex.StackTrace}");
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return View("Index", model);
            }
        }

        public async Task<IActionResult> ThanhCong(int id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefaultAsync(h => h.id_hoa_don == id);

            if (hoaDon == null) return NotFound();

            return View(hoaDon);
        }
    }
}