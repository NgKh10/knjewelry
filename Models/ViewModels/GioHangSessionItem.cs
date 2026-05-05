namespace knjewelry.Models.ViewModels
{
    public class GioHangSessionItem
    {
        public int SanPhamId { get; set; }
        public string TenSanPham { get; set; }
        public decimal DonGia { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => DonGia * SoLuong;
    }
}