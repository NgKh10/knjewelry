using Jewelry.Entities;
using Jewelry.Repository.EFCore;

namespace Jewelry.Services
{
    public class LoaiSanPhamService : ILoaiSanPhamService
    {
        private readonly ILoaiSanPhamRepository _repository;

        public LoaiSanPhamService(ILoaiSanPhamRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<LoaiSanPham> Items, int TotalCount)> SearchAsync(
            string? keyword, int? idLoaiCha, int page, int pageSize)
        {
            return await _repository.SearchAsync(keyword, idLoaiCha, page, pageSize);
        }

        public async Task<LoaiSanPham?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<LoaiSanPham> CreateAsync(LoaiSanPham loaiSanPham)
        {
            var exists = await _repository.GetByNameAsync(loaiSanPham.ten_loai);
            if (exists != null)
                throw new Exception("Tên danh mục đã tồn tại!");

            return await _repository.AddAsync(loaiSanPham);
        }

        public async Task UpdateAsync(LoaiSanPham loaiSanPham)
        {
            var existing = await _repository.GetByIdAsync(loaiSanPham.id_loai_sp);
            if (existing == null)
                throw new Exception("Danh mục không tồn tại!");

            var nameExists = await _repository.GetByNameAsync(loaiSanPham.ten_loai);
            if (nameExists != null && nameExists.id_loai_sp != loaiSanPham.id_loai_sp)
                throw new Exception("Tên danh mục đã tồn tại!");

            existing.ten_loai = loaiSanPham.ten_loai;
            existing.id_loai_cha = loaiSanPham.id_loai_cha;
            existing.thu_tu = loaiSanPham.thu_tu;
            existing.mo_ta = loaiSanPham.mo_ta;

            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Danh mục không tồn tại!");

            if (await _repository.HasChildrenAsync(id))
                throw new Exception("Không thể xóa: Đang có danh mục con!");

            if (await _repository.HasProductsAsync(id))
                throw new Exception("Không thể xóa: Đang có sản phẩm!");

            await _repository.DeleteByIdAsync(id);
        }
    }
}