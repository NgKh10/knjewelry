using System.ComponentModel.DataAnnotations;

namespace knjewelry.Models.ViewModels
{
    public class GioHangViewModel
    {
        public List<GioHangItemViewModel> DanhSachSanPham { get; set; } = new List<GioHangItemViewModel>();

        [DisplayFormat(DataFormatString = "{0:N0}₫")]
        public decimal TongTienHang { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}₫")]
        public decimal SoGiamGia { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}₫")]
        public decimal PhiVanChuyen { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}₫")]
        public decimal TongTien { get; set; }

        public int TongSoLuong { get; set; }
        public string MaGiamGia { get; set; }

        public bool DuocMienPhiShip => PhiVanChuyen == 0;
        public decimal CanThemDeMienPhiShip => TongTienHang >= 150000 ? 0 : 150000 - TongTienHang;

        public void TinhTong()
        {
            TongTienHang = DanhSachSanPham.Sum(i => i.ThanhTien);
            TongSoLuong = DanhSachSanPham.Sum(i => i.SoLuong);
            PhiVanChuyen = TongTienHang >= 150000 ? 0 : 30000;
            TongTien = TongTienHang - SoGiamGia + PhiVanChuyen;
        }
    }

    public class GioHangItemViewModel
    {
        public int IdChiTietGioHang { get; set; }
        public int IdSanPham { get; set; }
        public string TenSanPham { get; set; }
        public string DuongDanAnh { get; set; }
        public int? IdBienThe { get; set; }
        public string KichCo { get; set; }
        public string MauSac { get; set; }

        [Range(1, 99, ErrorMessage = "Số lượng từ 1-99")]
        public int SoLuong { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}₫")]
        public decimal DonGia { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}₫")]
        public decimal ThanhTien { get; set; }

        public int TonKho { get; set; }
        public bool HetHang => SoLuong > TonKho;
        public string TenBienThe => string.IsNullOrEmpty(KichCo) && string.IsNullOrEmpty(MauSac)
            ? "Mặc định"
            : $"{KichCo} {MauSac}".Trim();
    }

    public class ThemVaoGioViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn sản phẩm")]
        public int SanPhamId { get; set; }

        public int? BienTheId { get; set; }

        public string KichCo { get; set; }
        public string MauSac { get; set; }

        [Range(1, 99, ErrorMessage = "Số lượng từ 1-99")]
        public int SoLuong { get; set; } = 1;
    }
}