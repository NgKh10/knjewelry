using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public interface ITaiKhoanRepository : IGenericRepository<NguoiDung>
    {
        Task<NguoiDung> LoginAsync(string tenDangNhap, string matKhau);
        Task<bool> UsernameExistsAsync(string tenDangNhap);
        Task<bool> EmailExistsAsync(string email);
        Task<NguoiDung> GetByEmailAsync(string email);
    }
}