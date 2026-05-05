using knjewelry.Models.Entities;

namespace knjewelry.Models.ViewModels
{
    public class ChiTietSanPhamViewModel
    {
        public SanPham SanPham { get; set; }
        public List<HinhAnhSanPham> DanhSachHinhAnh { get; set; }
        public List<BienThe> DanhSachBienThe { get; set; }
        public List<string> DanhSachKichCo { get; set; }
        public List<string> DanhSachMauSac { get; set; }
        public IEnumerable<SanPham> SanPhamLienQuan { get; set; }

        public decimal GiaHienTai => SanPham.gia_khuyen_mai ?? SanPham.gia;
        public bool DangGiamGia => SanPham.gia_khuyen_mai.HasValue && SanPham.gia_khuyen_mai < SanPham.gia;

        public int PhanTramGiam => DangGiamGia
            ? (int)((SanPham.gia - SanPham.gia_khuyen_mai.Value) / SanPham.gia * 100)
            : 0;

        public string MoTaHtml => SanPham.mo_ta ?? "";
        public string TenChatLieu => SanPham.ChatLieu?.ten_chat_lieu ?? "";
        public string TenDanhMuc => SanPham.LoaiSanPham?.ten_loai ?? "";
    }
}