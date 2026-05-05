using knjewelry.Models.Entities;

namespace knjewelry.Models.ViewModels
{
    public class DanhSachSanPhamViewModel
    {
        public IEnumerable<SanPham> DanhSachSanPham { get; set; }
        public int TrangHienTai { get; set; }
        public int TongTrang { get; set; }
        public int? LoaiId { get; set; }
        public string TuKhoa { get; set; }

        public bool CoTrangTruoc => TrangHienTai > 1;
        public bool CoTrangSau => TrangHienTai < TongTrang;

        public List<int> GetDanhSachTrang(int maxHienThi = 5)
        {
            var pages = new List<int>();
            int batDau = Math.Max(1, TrangHienTai - maxHienThi / 2);
            int ketThuc = Math.Min(TongTrang, batDau + maxHienThi - 1);

            for (int i = batDau; i <= ketThuc; i++)
            {
                pages.Add(i);
            }
            return pages;
        }
    }
}