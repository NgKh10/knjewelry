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

        /// <summary>
        /// Kiểm tra thông tin đăng nhập và trả về tài khoản nếu hợp lệ.
        /// Chỉ cho phép đăng nhập đối với các tài khoản đang hoạt động.
        /// </summary>
        /// <param name="tenDangNhap">Tên đăng nhập.</param>
        /// <param name="matKhau">Mật khẩu.</param>
        /// <returns>Thông tin người dùng nếu đăng nhập thành công, ngược lại trả về null.</returns>
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

        /// <summary>
        /// Kiểm tra xem tên đăng nhập đã tồn tại trong hệ thống hay chưa.
        /// </summary>
        /// <param name="tenDangNhap">Tên đăng nhập cần kiểm tra.</param>
        /// <returns>
        /// True nếu tên đăng nhập đã tồn tại, ngược lại False.
        /// </returns>
        public async Task<bool> UsernameExistsAsync(string tenDangNhap)
        {
            return await _dbSet.AnyAsync(u => u.ten_dang_nhap == tenDangNhap);
        }

        /// <summary>
        /// Kiểm tra xem địa chỉ email đã được sử dụng hay chưa.
        /// </summary>
        /// <param name="email">Địa chỉ email cần kiểm tra.</param>
        /// <returns>
        /// True nếu email đã tồn tại, ngược lại False.
        /// </returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.email == email);
        }

        /// <summary>
        /// Tìm kiếm và lấy thông tin người dùng theo địa chỉ email.
        /// </summary>
        /// <param name="email">Địa chỉ email của người dùng.</param>
        /// <returns>
        /// Thông tin người dùng tương ứng với email, trả về null nếu không tìm thấy.
        /// </returns>
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