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

            var siblings = await _repository.GetBySanPhamAsync(entity.id_san_pham);

            // Thứ tự thêm mới không được trùng với ảnh đã có
            if (siblings.Any(i => i.thu_tu == entity.thu_tu))
                throw new Exception($"Thứ tự {entity.thu_tu} đã tồn tại cho sản phẩm này. Vui lòng chọn thứ tự khác!");

            if (entity.la_chinh)
            {
                foreach (var img in siblings.Where(i => i.la_chinh))
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

            var siblings = await _repository.GetBySanPhamAsync(existing.id_san_pham);

            if (entity.la_chinh && !existing.la_chinh)
            {
                // Đặt làm ảnh chính: chuyển thứ tự thành 1, ảnh chính cũ nhận thứ tự hiện tại
                var oldMain = siblings.FirstOrDefault(i => i.la_chinh && i.id_hinh_anh != id);
                if (oldMain != null)
                {
                    oldMain.la_chinh = false;
                    oldMain.thu_tu = existing.thu_tu;
                    await _repository.UpdateAsync(oldMain);
                }
                existing.thu_tu = 1;
            }
            else if (entity.thu_tu != existing.thu_tu)
            {
                // Nếu thứ tự mới đã tồn tại, hoán đổi với ảnh đó
                var conflict = siblings.FirstOrDefault(i => i.thu_tu == entity.thu_tu && i.id_hinh_anh != id);
                if (conflict != null)
                {
                    conflict.thu_tu = existing.thu_tu;
                    await _repository.UpdateAsync(conflict);
                }
                existing.thu_tu = entity.thu_tu;
            }

            existing.duong_dan = entity.duong_dan;
            existing.la_chinh = entity.la_chinh;

            await _repository.UpdateAsync(existing);
        }

        public async Task SetMainAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                throw new Exception("Không tìm thấy hình ảnh!");

            var siblings = await _repository.GetBySanPhamAsync(item.id_san_pham);

            // Hoán đổi thứ tự với ảnh chính cũ
            var previousMain = siblings.FirstOrDefault(i => i.la_chinh && i.id_hinh_anh != id);
            if (previousMain != null)
            {
                previousMain.la_chinh = false;
                previousMain.thu_tu = item.thu_tu;
                await _repository.UpdateAsync(previousMain);
            }

            item.la_chinh = true;
            item.thu_tu = 1;
            await _repository.UpdateAsync(item);
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
