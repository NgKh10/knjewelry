using knjewelry.Models.Entities;

namespace knjewelry.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<SanPham> SanPhamMoi { get; set; }
        public IEnumerable<SanPham> SanPhamHot { get; set; }
        public IEnumerable<LoaiSanPham> DanhMucNoiBat { get; set; }
        public int SoLuongGioHang { get; set; }

        // Constructor khởi tạo giá trị mặc định
        public HomeViewModel()
        {
            SanPhamMoi = new List<SanPham>();
            SanPhamHot = new List<SanPham>();
            DanhMucNoiBat = new List<LoaiSanPham>();
            SoLuongGioHang = 0;
        }
    }
}