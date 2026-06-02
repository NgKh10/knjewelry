namespace knjewelry.Models.ViewModels
{
    public class GioHangSessionItem
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; } = "";
        public decimal DonGia { get; set; }
        public string HinhAnh { get; set; } = "";
        public int SoLuong { get; set; }
        public string Size { get; set; } = "";      
        public string MauSac { get; set; } = "";    
        public decimal ThanhTien => DonGia * SoLuong;
        public string PhanLoai => string.IsNullOrEmpty(Size) && string.IsNullOrEmpty(MauSac)
            ? "Mặc định"
            : $"{Size} {MauSac}".Trim();
    }
}