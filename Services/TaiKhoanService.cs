using knjewelry.Models.Entities;
using knjewelry.Models.ViewModels;
using knjewelry.Repository;
using knjewelry.Services;

namespace knjewelry.Services
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly ITaiKhoanRepository _taiKhoanRepository;

        public TaiKhoanService(ITaiKhoanRepository taiKhoanRepository)
        {
            _taiKhoanRepository = taiKhoanRepository;
        }

        public async Task<NguoiDung> DangNhapAsync(string tenDangNhap, string matKhau)
        {
            return await _taiKhoanRepository.LoginAsync(tenDangNhap, matKhau);
        }

        public async Task<NguoiDung> DangKyAsync(DangKyViewModel model)
        {
            // Kiểm tra tên đăng nhập đã tồn tại
            if (await _taiKhoanRepository.UsernameExistsAsync(model.TenDangNhap))
                throw new Exception("Tên đăng nhập đã tồn tại");

            // Kiểm tra email đã tồn tại
            if (await _taiKhoanRepository.EmailExistsAsync(model.Email))
                throw new Exception("Email đã được sử dụng");

            var user = new NguoiDung
            {
                ho_ten = model.HoTen,
                email = model.Email,
                ten_dang_nhap = model.TenDangNhap,
                mat_khau = model.MatKhau,
                so_dien_thoai = model.SoDienThoai ?? "",
                dia_chi = model.DiaChi ?? "",
                vai_tro = "khach_hang",
                trang_thai = 1,
                ngay_tao = DateTime.Now
            };

            await _taiKhoanRepository.AddAsync(user);
            await _taiKhoanRepository.SaveChangesAsync();

            return user;  // Trả về user vừa tạo để đăng nhập tự động
        }

        public async Task<NguoiDung> GetThongTinAsync(int userId)
        {
            return await _taiKhoanRepository.GetByIdAsync(userId);
        }

        public async Task<bool> CapNhatThongTinAsync(int userId, CapNhatThongTinViewModel model)
        {
            var user = await _taiKhoanRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.ho_ten = model.HoTen;
            user.so_dien_thoai = model.SoDienThoai;
            user.dia_chi = model.DiaChi;

            _taiKhoanRepository.Update(user);
            await _taiKhoanRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DoiMatKhauAsync(int userId, string matKhauCu, string matKhauMoi)
        {
            var user = await _taiKhoanRepository.GetByIdAsync(userId);
            if (user == null || user.mat_khau != matKhauCu)
                return false;

            user.mat_khau = matKhauMoi;
            _taiKhoanRepository.Update(user);
            await _taiKhoanRepository.SaveChangesAsync();
            return true;
        }
    }
}