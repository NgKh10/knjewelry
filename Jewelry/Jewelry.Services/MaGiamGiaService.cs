using Jewelry.Entities;
using Jewelry.Repository.EFCore;

namespace Jewelry.Services
{
    public class MaGiamGiaService : IMaGiamGiaService
    {
        private readonly IMaGiamGiaRepository _repository;

        public MaGiamGiaService(IMaGiamGiaRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<MaGiamGia> Items, int TotalCount)> SearchAsync(
            string? keyword, string? loaiGiam, byte? trangThai, int page, int pageSize, string? sortOrder = null)
        {
            return await _repository.SearchAsync(keyword, loaiGiam, trangThai, page, pageSize, sortOrder);
        }

        public async Task<MaGiamGia?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<MaGiamGia?> GetByCodeAsync(string code)
        {
            return await _repository.GetByCodeAsync(code);
        }

        public async Task<MaGiamGia> CreateAsync(MaGiamGia entity)
        {
            if (string.IsNullOrWhiteSpace(entity.ma_code))
                throw new Exception("Mã code không được để trống!");

            var exists = await _repository.GetByCodeAsync(entity.ma_code);
            if (exists != null)
                throw new Exception("Mã code đã tồn tại!");

            if (entity.loai_giam != "phan_tram" && entity.loai_giam != "tien_mat")
                throw new Exception("Loại giảm không hợp lệ! Chỉ chấp nhận 'phan_tram' hoặc 'tien_mat'.");

            if (entity.gia_tri <= 0)
                throw new Exception("Giá trị giảm phải lớn hơn 0!");

            if (entity.loai_giam == "phan_tram" && entity.gia_tri > 100)
                throw new Exception("Phần trăm giảm không được vượt quá 100%!");

            entity.da_su_dung = 0;
            entity.trang_thai = (entity.ngay_ket_thuc.HasValue && entity.ngay_ket_thuc < DateTime.Now) ? (byte)0 : (byte)1;

            return await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(int id, MaGiamGia entity)
        {
            if (string.IsNullOrWhiteSpace(entity.ma_code))
                throw new Exception("Mã code không được để trống!");

            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Không tìm thấy mã giảm giá!");

            var duplicate = await _repository.GetByCodeAsync(entity.ma_code);
            if (duplicate != null && duplicate.id_ma_giam_gia != id)
                throw new Exception("Mã code đã tồn tại!");

            if (entity.gia_tri <= 0)
                throw new Exception("Giá trị giảm phải lớn hơn 0!");

            existing.ma_code = entity.ma_code;
            existing.mo_ta = entity.mo_ta;
            existing.loai_giam = entity.loai_giam;
            existing.gia_tri = entity.gia_tri;
            existing.giam_toi_da = entity.giam_toi_da;
            existing.don_hang_toi_thieu = entity.don_hang_toi_thieu;
            existing.so_luong = entity.so_luong;
            existing.ngay_bat_dau = entity.ngay_bat_dau;
            existing.ngay_ket_thuc = entity.ngay_ket_thuc;

            // Nếu ngày hết hạn đã qua thì tự động ngừng, bất kể người dùng đặt gì
            if (entity.ngay_ket_thuc.HasValue && entity.ngay_ket_thuc < DateTime.Now)
                existing.trang_thai = 0;
            else
                existing.trang_thai = entity.trang_thai;

            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            if (item == null)
                throw new Exception("Không tìm thấy mã giảm giá!");

            if (await _repository.HasOrdersAsync(id))
                throw new Exception("Không thể xóa: Mã đã được sử dụng trong đơn hàng!");

            await _repository.DeleteAsync(item);
        }
    }
}
