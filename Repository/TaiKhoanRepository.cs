using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public class TaiKhoanRepository : GenericRepository<NguoiDung>, ITaiKhoanRepository
    {
        public TaiKhoanRepository(TrangSucBacContext context) : base(context)
        {
        }

        public async Task<NguoiDung> LoginAsync(string tenDangNhap, string matKhau)
        {
            // Sửa: Thêm AsNoTracking() và xử lý null
            return await _dbSet
                .AsNoTracking()
                .Where(u => u.ten_dang_nhap == tenDangNhap && u.mat_khau == matKhau && u.trang_thai == 1)
                .Select(u => new NguoiDung
                {
                    id_nguoi_dung = u.id_nguoi_dung,
                    ho_ten = u.ho_ten ?? "",
                    email = u.email ?? "",
                    ten_dang_nhap = u.ten_dang_nhap ?? "",
                    mat_khau = u.mat_khau ?? "",
                    so_dien_thoai = u.so_dien_thoai ?? "",
                    dia_chi = u.dia_chi ?? "",
                    vai_tro = u.vai_tro ?? "khach_hang",
                    trang_thai = u.trang_thai,
                    ngay_tao = u.ngay_tao
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UsernameExistsAsync(string tenDangNhap)
        {
            return await _dbSet.AnyAsync(u => u.ten_dang_nhap == tenDangNhap);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.email == email);
        }

        public async Task<NguoiDung> GetByEmailAsync(string email)
        {
            return await _dbSet
                .AsNoTracking()
                .Select(u => new NguoiDung
                {
                    id_nguoi_dung = u.id_nguoi_dung,
                    ho_ten = u.ho_ten ?? "",
                    email = u.email ?? "",
                    ten_dang_nhap = u.ten_dang_nhap ?? "",
                    mat_khau = u.mat_khau ?? "",
                    so_dien_thoai = u.so_dien_thoai ?? "",
                    dia_chi = u.dia_chi ?? "",
                    vai_tro = u.vai_tro ?? "khach_hang",
                    trang_thai = u.trang_thai,
                    ngay_tao = u.ngay_tao
                })
                .FirstOrDefaultAsync(u => u.email == email);
        }
    }
}