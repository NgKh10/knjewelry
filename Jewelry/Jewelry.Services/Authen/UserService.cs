using Jewelry.Entities;
using Jewelry.Repository.EFCore;

namespace Jewelry.Services.Authen
{
    public interface INguoiDungService
    {
        Task<NguoiDung?> Authenticate(string tenDangNhap, string matKhau);
        Task<List<NguoiDung>> GetAll();
        Task<NguoiDung?> GetById(int id);
        Task<NguoiDung?> GetByUsername(string username);
        Task<NguoiDung> Create(NguoiDung user, string matKhau);
        Task Update(NguoiDung user);
        Task Delete(int id);
        Task<(List<NguoiDung> Users, int TotalCount)> Search(string? keyword, int page, int pageSize);
    }

    public class NguoiDungService : INguoiDungService
    {
        private readonly INguoiDungRepository _nguoiDungRepository;

        public NguoiDungService(INguoiDungRepository nguoiDungRepository)
        {
            _nguoiDungRepository = nguoiDungRepository;
        }

        public async Task<NguoiDung?> Authenticate(string tenDangNhap, string matKhau)
        {
            return await _nguoiDungRepository.LoginAsync(tenDangNhap, matKhau);
        }

        public async Task<List<NguoiDung>> GetAll()
        {
            var items = await _nguoiDungRepository.GetAllAsync();
            return items.ToList();
        }

        public async Task<NguoiDung?> GetById(int id)
        {
            return await _nguoiDungRepository.GetByIdAsync(id);
        }

        public async Task<NguoiDung?> GetByUsername(string username)
        {
            return await _nguoiDungRepository.GetByUsernameAsync(username);
        }

        public async Task<NguoiDung> Create(NguoiDung user, string matKhau)
        {
            user.mat_khau = matKhau;
            user.ngay_tao = DateTime.Now;
            user.trang_thai = 1;
            user.vai_tro = "khach_hang";

            await _nguoiDungRepository.AddAsync(user);
            return user;
        }

        public async Task Update(NguoiDung user)
        {
            await _nguoiDungRepository.UpdateAsync(user);
        }

        public async Task Delete(int id)
        {
            var user = await _nguoiDungRepository.GetByIdAsync(id);
            if (user != null)
                await _nguoiDungRepository.DeleteAsync(user);
        }

        public async Task<(List<NguoiDung> Users, int TotalCount)> Search(string? keyword, int page, int pageSize)
        {
            return await _nguoiDungRepository.SearchAsync(keyword, null, null, page, pageSize);
        }
    }
}
