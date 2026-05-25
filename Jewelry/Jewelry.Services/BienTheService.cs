using Jewelry.Entities;
using Jewelry.Repository.EFCore;

namespace Jewelry.Services
{
    public class BienTheService : IBienTheService
    {
        private readonly IBienTheRepository _repository;

        public BienTheService(IBienTheRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<BienThe> Items, int TotalCount)> SearchAsync(
            string? tenSanPham, string? kichCo, string? mauSac, int page, int pageSize, string? sortOrder = null)
        {
            return await _repository.SearchAsync(tenSanPham, kichCo, mauSac, page, pageSize, sortOrder);
        }

        public async Task<BienThe?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<BienThe>> GetBySanPhamAsync(int idSanPham)
        {
            return await _repository.GetBySanPhamAsync(idSanPham);
        }

        public async Task<BienThe> CreateAsync(BienThe entity)
        {
            if (entity.so_luong_ton < 0)
                throw new Exception("Số lượng tồn kho không được âm!");

            if (entity.gia_them < 0)
                throw new Exception("Giá thêm không được âm!");

            return await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, BienThe entity)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Không tìm thấy biến thể!");

            if (entity.so_luong_ton < 0)
                throw new Exception("Số lượng tồn kho không được âm!");

            if (entity.gia_them < 0)
                throw new Exception("Giá thêm không được âm!");

            existing.kich_co = entity.kich_co;
            existing.mau_sac = entity.mau_sac;
            existing.so_luong_ton = entity.so_luong_ton;
            existing.gia_them = entity.gia_them;

            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                throw new Exception("Không tìm thấy biến thể!");

            await _repository.DeleteAsync(item);
        }
    }
}
