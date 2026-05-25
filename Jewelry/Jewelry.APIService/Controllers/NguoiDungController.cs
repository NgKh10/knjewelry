using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Jewelry.Services.Authen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NguoiDungController : ControllerBase
{
    private readonly INguoiDungRepository _repository;
    private readonly INguoiDungService _userService;

    public NguoiDungController(INguoiDungRepository repository, INguoiDungService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    /// <summary>GET /api/nguoidung/me</summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _repository.GetByIdAsync(int.Parse(userId));
        if (user == null) return NotFound();

        return Ok(new
        {
            user.id_nguoi_dung,
            user.ho_ten,
            user.email,
            user.ten_dang_nhap,
            user.so_dien_thoai,
            user.dia_chi,
            user.vai_tro,
            user.trang_thai,
            user.ngay_tao
        });
    }

    /// <summary>GET /api/nguoidung?keyword=&vaiTro=&trangThai=&page=&pageSize=&sortOrder=</summary>
    [HttpGet]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] string? vaiTro,
        [FromQuery] byte? trangThai,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortOrder = null)
    {
        var (users, totalCount) = await _repository.SearchAsync(keyword, vaiTro, trangThai, page, pageSize, sortOrder);

        return Ok(new
        {
            items = users,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            page,
            pageSize
        });
    }

    /// <summary>GET /api/nguoidung/{id}</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null) return NotFound(new { message = "Không tìm thấy người dùng!" });

        return Ok(new
        {
            user.id_nguoi_dung,
            user.ho_ten,
            user.email,
            user.ten_dang_nhap,
            user.so_dien_thoai,
            user.dia_chi,
            user.vai_tro,
            user.trang_thai,
            user.ngay_tao
        });
    }

    /// <summary>POST /api/nguoidung - Tạo người dùng mới (admin)</summary>
    [HttpPost]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ten_dang_nhap) || string.IsNullOrWhiteSpace(request.mat_khau))
            return BadRequest(new { message = "Tên đăng nhập và mật khẩu không được để trống!" });

        if (request.mat_khau.Length < 6)
            return BadRequest(new { message = "Mật khẩu phải có ít nhất 6 ký tự!" });

        var existing = await _repository.GetByUsernameAsync(request.ten_dang_nhap);
        if (existing != null)
            return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });

        if (!string.IsNullOrEmpty(request.email))
        {
            var emailExists = await _repository.GetByEmailAsync(request.email);
            if (emailExists != null)
                return BadRequest(new { message = "Email đã được sử dụng!" });
        }

        var user = new NguoiDung
        {
            ten_dang_nhap = request.ten_dang_nhap,
            ho_ten = string.IsNullOrEmpty(request.ho_ten) ? request.ten_dang_nhap : request.ho_ten,
            email = request.email ?? "",
            so_dien_thoai = request.so_dien_thoai,
            dia_chi = request.dia_chi,
            vai_tro = request.vai_tro ?? "khach_hang",
            trang_thai = 1,
            ngay_tao = DateTime.Now
        };

        var created = await _userService.Create(user, request.mat_khau);
        return Ok(new { created.id_nguoi_dung, created.ten_dang_nhap, message = "Tạo tài khoản thành công!" });
    }

    /// <summary>PUT /api/nguoidung/{id}</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Không tìm thấy người dùng!" });

        if (!string.IsNullOrEmpty(request.email) && request.email != existing.email)
        {
            var emailExists = await _repository.GetByEmailAsync(request.email);
            if (emailExists != null && emailExists.id_nguoi_dung != id)
                return BadRequest(new { message = "Email đã được sử dụng!" });
        }

        existing.ho_ten = request.ho_ten ?? existing.ho_ten;
        existing.email = request.email ?? existing.email;
        existing.so_dien_thoai = request.so_dien_thoai ?? existing.so_dien_thoai;
        existing.dia_chi = request.dia_chi ?? existing.dia_chi;
        existing.vai_tro = request.vai_tro ?? existing.vai_tro;
        existing.trang_thai = request.trang_thai ?? existing.trang_thai;

        if (!string.IsNullOrEmpty(request.mat_khau))
            await _userService.Update(existing);
        else
            await _repository.UpdateAsync(existing);

        return Ok(new { message = "Cập nhật thành công!" });
    }

    /// <summary>DELETE /api/nguoidung/{id}</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        var currentUserId = User.FindFirst("id")?.Value;
        if (currentUserId == id.ToString())
            return BadRequest(new { message = "Không thể xóa tài khoản đang đăng nhập!" });

        var user = await _repository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = "Không tìm thấy người dùng!" });

        await _repository.DeleteAsync(user);
        return Ok(new { message = "Xóa người dùng thành công!" });
    }
}

public class CreateUserRequest
{
    public string ten_dang_nhap { get; set; } = "";
    public string mat_khau { get; set; } = "";
    public string? ho_ten { get; set; }
    public string? email { get; set; }
    public string? so_dien_thoai { get; set; }
    public string? dia_chi { get; set; }
    public string? vai_tro { get; set; }
}

public class UpdateUserRequest
{
    public string? ho_ten { get; set; }
    public string? email { get; set; }
    public string? so_dien_thoai { get; set; }
    public string? dia_chi { get; set; }
    public string? vai_tro { get; set; }
    public byte? trang_thai { get; set; }
    public string? mat_khau { get; set; }
}
