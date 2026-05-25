using Jewelry.Entities;
using Jewelry.Repository.EFCore;

namespace Jewelry.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ISanPhamRepository _repository;

        public SanPhamService(ISanPhamRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<SanPham> Products, int TotalCount)> SearchAsync(
            string? keyword, int? loaiId, decimal? giaTu, decimal? giaDen, int page, int pageSize, string? sortOrder = null)
        {
            return await _repository.SearchAsync(keyword, loaiId, giaTu, giaDen, page, pageSize, sortOrder);
        }

        public async Task<SanPham?> GetByIdAsync(int id)
        {
            return await _repository.GetWithDetailsAsync(id);
        }

        public async Task<SanPham> CreateAsync(SanPham sanPham)
        {
            var exists = await _repository.IsNameExistsAsync(sanPham.ten_sp);
            if (exists)
                throw new Exception("Tên sản phẩm đã tồn tại!");

            sanPham.ngay_tao = DateTime.Now;
            sanPham.trang_thai = 1;
            return await _repository.AddAsync(sanPham);
        }

        public async Task UpdateAsync(SanPham sanPham)
        {
            var existing = await _repository.GetByIdAsync(sanPham.id_san_pham);
            if (existing == null)
                throw new Exception("Sản phẩm không tồn tại!");

            var nameExists = await _repository.IsNameExistsAsync(sanPham.ten_sp, sanPham.id_san_pham);
            if (nameExists)
                throw new Exception("Tên sản phẩm đã tồn tại!");

            existing.ten_sp = sanPham.ten_sp;
            existing.gia = sanPham.gia;
            existing.gia_khuyen_mai = sanPham.gia_khuyen_mai;
            existing.id_loai_sp = sanPham.id_loai_sp;
            existing.id_chat_lieu = sanPham.id_chat_lieu;
            existing.trong_luong = sanPham.trong_luong;
            existing.mo_ta = sanPham.mo_ta;
            existing.trang_thai = sanPham.trang_thai;

            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Sản phẩm không tồn tại!");

            existing.trang_thai = 0; // Soft delete
            await _repository.UpdateAsync(existing);
        }
    }
}