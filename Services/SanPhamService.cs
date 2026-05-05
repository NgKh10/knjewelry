using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;
using knjewelry.Repository;

namespace knjewelry.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ISanPhamRepository _sanPhamRepository;

        public SanPhamService(ISanPhamRepository sanPhamRepository)
        {
            _sanPhamRepository = sanPhamRepository;
        }

        public async Task<DanhSachSanPhamViewModel> GetDanhSachSanPhamAsync(int? loaiId, string search, int page, int pageSize = 12)
        {
            var products = await _sanPhamRepository.SearchProductsAsync(search, loaiId);
            var totalItems = products.Count();
            var pagedProducts = products.Skip((page - 1) * pageSize).Take(pageSize);

            return new DanhSachSanPhamViewModel
            {
                DanhSachSanPham = pagedProducts,
                TrangHienTai = page,
                TongTrang = (int)Math.Ceiling(totalItems / (double)pageSize),
                LoaiId = loaiId,
                TuKhoa = search
            };
        }

        public async Task<ChiTietSanPhamViewModel> GetChiTietSanPhamAsync(int id)
        {
            var product = await _sanPhamRepository.GetProductWithDetailsAsync(id);
            if (product == null) return null;

            var relatedProducts = await _sanPhamRepository.GetProductsByCategoryAsync(product.id_loai_sp);
            relatedProducts = relatedProducts.Where(p => p.id_san_pham != id).Take(6);

            return new ChiTietSanPhamViewModel
            {
                SanPham = product,
                DanhSachHinhAnh = product.HinhAnhs?.ToList() ?? new List<HinhAnhSanPham>(),
                DanhSachBienThe = product.BienThes?.ToList() ?? new List<BienThe>(),
                DanhSachKichCo = product.BienThes?.Where(v => !string.IsNullOrEmpty(v.kich_co)).Select(v => v.kich_co).Distinct().ToList() ?? new List<string>(),
                DanhSachMauSac = product.BienThes?.Where(v => !string.IsNullOrEmpty(v.mau_sac)).Select(v => v.mau_sac).Distinct().ToList() ?? new List<string>(),
                SanPhamLienQuan = relatedProducts.ToList()
            };
        }

        public async Task<IEnumerable<SanPham>> GetSanPhamMoiAsync(int take)
        {
            var result = await _sanPhamRepository.GetNewProductsAsync(take);
            return result ?? new List<SanPham>();  // Trả về list rỗng nếu null
        }

        public async Task<IEnumerable<SanPham>> GetSanPhamHotAsync(int take)
        {
            var result = await _sanPhamRepository.GetHotProductsAsync(take);
            return result ?? new List<SanPham>();
        }
    }
}