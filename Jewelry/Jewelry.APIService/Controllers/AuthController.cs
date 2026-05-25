using Jewelry.Common.Auth;
using Jewelry.Services.Authen;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly INguoiDungService _userService;
    private readonly TokenHelper _tokenHelper;

    public AuthController(INguoiDungService userService, TokenHelper tokenHelper)
    {
        _userService = userService;
        _tokenHelper = tokenHelper;
    }

    /// <summary>POST /api/auth/login</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.ten_dang_nhap) || string.IsNullOrEmpty(request.mat_khau))
            return BadRequest(new { message = "Tên đăng nhập và mật khẩu không được để trống" });

        var user = await _userService.Authenticate(request.ten_dang_nhap, request.mat_khau);

        if (user == null)
            return Unauthorized(new { message = "Sai tên đăng nhập hoặc mật khẩu" });

        if (user.trang_thai != 1)
            return Unauthorized(new { message = "Tài khoản đã bị khóa" });

        var token = _tokenHelper.GenerateToken(
            user.id_nguoi_dung.ToString(),
            user.ten_dang_nhap,
            user.vai_tro);

        return Ok(new
        {
            token,
            userId = user.id_nguoi_dung,
            username = user.ten_dang_nhap,
            name = user.ho_ten,
            email = user.email,
            vai_tro = user.vai_tro
        });
    }

    /// <summary>POST /api/auth/register</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request == null)
            return BadRequest(new { message = "Dữ liệu không hợp lệ" });

        if (string.IsNullOrEmpty(request.ten_dang_nhap) || string.IsNullOrEmpty(request.mat_khau))
            return BadRequest(new { message = "Tên đăng nhập và mật khẩu không được để trống" });

        if (request.mat_khau.Length < 6)
            return BadRequest(new { message = "Mật khẩu phải có ít nhất 6 ký tự" });

        var existing = await _userService.GetByUsername(request.ten_dang_nhap);
        if (existing != null)
            return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });

        var user = new Jewelry.Entities.NguoiDung
        {
            ten_dang_nhap = request.ten_dang_nhap,
            ho_ten = string.IsNullOrEmpty(request.ho_ten) ? request.ten_dang_nhap : request.ho_ten,
            email = request.email ?? "",
            so_dien_thoai = request.so_dien_thoai,
            dia_chi = request.dia_chi,
            vai_tro = "khach_hang",
            trang_thai = 1,
            ngay_tao = DateTime.Now
        };

        var createdUser = await _userService.Create(user, request.mat_khau);
        return Ok(new { message = "Đăng ký thành công", id = createdUser.id_nguoi_dung });
    }
}

public class LoginRequest
{
    public string ten_dang_nhap { get; set; } = "";
    public string mat_khau { get; set; } = "";
}

public class RegisterRequest
{
    public string ten_dang_nhap { get; set; } = "";
    public string mat_khau { get; set; } = "";
    public string? ho_ten { get; set; }
    public string? email { get; set; }
    public string? so_dien_thoai { get; set; }
    public string? dia_chi { get; set; }
}
