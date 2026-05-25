using Microsoft.EntityFrameworkCore;
using Jewelry.Entities;

namespace Jewelry.Repository.EFCore
{
    public interface INguoiDungRepository : IRepository<NguoiDung>
    {
        Task<NguoiDung?> LoginAsync(string tenDangNhap, string matKhau);
        Task<NguoiDung?> GetByUsernameAsync(string username);
        Task<NguoiDung?> GetByEmailAsync(string email);
        Task<(List<NguoiDung> Users, int TotalCount)> SearchAsync(
            string? keyword, string? vaiTro, byte? trangThai, int page, int pageSize, string? sortOrder = null);
    }

    public class NguoiDungRepository : Repository<NguoiDung>, INguoiDungRepository
    {
        public NguoiDungRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<NguoiDung?> LoginAsync(string tenDangNhap, string matKhau)
        {
            return await _dbSet.FirstOrDefaultAsync(u =>
                u.ten_dang_nhap == tenDangNhap && u.mat_khau == matKhau && u.trang_thai == 1);
        }

        public async Task<NguoiDung?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.ten_dang_nhap == username);
        }

        public async Task<NguoiDung?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<(List<NguoiDung> Users, int TotalCount)> SearchAsync(
            string? keyword, string? vaiTro, byte? trangThai, int page, int pageSize, string? sortOrder = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(u =>
                    u.ten_dang_nhap.ToLower().Contains(keyword) ||
                    u.ho_ten.ToLower().Contains(keyword) ||
                    u.email.ToLower().Contains(keyword) ||
                    (u.so_dien_thoai != null && u.so_dien_thoai.Contains(keyword)));
            }

            if (!string.IsNullOrEmpty(vaiTro))
                query = query.Where(u => u.vai_tro == vaiTro);

            if (trangThai.HasValue)
                query = query.Where(u => u.trang_thai == trangThai.Value);

            var totalCount = await query.CountAsync();

            IQueryable<NguoiDung> orderedQuery = sortOrder == "desc"
                ? query.OrderByDescending(u => u.ho_ten)
                : sortOrder == "asc"
                    ? query.OrderBy(u => u.ho_ten)
                    : query.OrderByDescending(u => u.ngay_tao);

            var users = await orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new NguoiDung
                {
                    id_nguoi_dung = u.id_nguoi_dung,
                    ho_ten = u.ho_ten,
                    email = u.email,
                    ten_dang_nhap = u.ten_dang_nhap,
                    mat_khau = "",
                    so_dien_thoai = u.so_dien_thoai,
                    dia_chi = u.dia_chi,
                    vai_tro = u.vai_tro,
                    trang_thai = u.trang_thai,
                    ngay_tao = u.ngay_tao
                })
                .ToListAsync();

            return (users, totalCount);
        }
    }
}