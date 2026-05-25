using Jewelry.Entities;
using Jewelry.Repository.EFCore;

namespace Jewelry.Services
{
    public class HinhAnhSanPhamService : IHinhAnhSanPhamService
    {
        private readonly IHinhAnhSanPhamRepository _repository;

        public HinhAnhSanPhamService(IHinhAnhSanPhamRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<HinhAnhSanPham> Items, int TotalCount)> SearchAsync(
            int? idSanPham, bool? laChinhFilter, string? tenSanPham, int page, int pageSize, string? sortOrder = null)
        {
            return await _repository.SearchAsync(idSanPham, laChinhFilter, tenSanPham, page, pageSize, sortOrder);
        }

        public async Task<HinhAnhSanPham?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<HinhAnhSanPham>> GetBySanPhamAsync(int idSanPham)
        {
            return await _repository.GetBySanPhamAsync(idSanPham);
        }

        public async Task<HinhAnhSanPham> CreateAsync(HinhAnhSanPham entity)
        {
            if (string.IsNullOrWhiteSpace(entity.duong_dan))
                throw new Exception("Đường dẫn hình ảnh không được để trống!");

            if (entity.la_chinh)
            {
                var existing = await _repository.GetBySanPhamAsync(entity.id_san_pham);
                foreach (var img in existing.Where(i => i.la_chinh))
                {
                    img.la_chinh = false;
                    await _repository.UpdateAsync(img);
                }
            }

            return await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, HinhAnhSanPham entity)
        {
            if (string.IsNullOrWhiteSpace(entity.duong_dan))
                throw new Exception("Đường dẫn hình ảnh không được để trống!");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Không tìm thấy hình ảnh!");

            if (entity.la_chinh && !existing.la_chinh)
            {
                var siblings = await _repository.GetBySanPhamAsync(existing.id_san_pham);
                foreach (var img in siblings.Where(i => i.la_chinh && i.id_hinh_anh != id))
                {
                    img.la_chinh = false;
                    await _repository.UpdateAsync(img);
                }
            }

            existing.duong_dan = entity.duong_dan;
            existing.la_chinh = entity.la_chinh;
            existing.thu_tu = entity.thu_tu;

            await _repository.UpdateAsync(existing);
        }

        public async Task SetMainAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                throw new Exception("Không tìm thấy hình ảnh!");

            var siblings = await _repository.GetBySanPhamAsync(item.id_san_pham);
            foreach (var img in siblings)
            {
                img.la_chinh = (img.id_hinh_anh == id);
                await _repository.UpdateAsync(img);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                throw new Exception("Không tìm thấy hình ảnh!");

            await _repository.DeleteAsync(item);
        }
    }
}
