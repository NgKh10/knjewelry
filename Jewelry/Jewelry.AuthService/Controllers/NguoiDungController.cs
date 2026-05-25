using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NguoiDungController : ControllerBase
{
    private readonly INguoiDungRepository _repository;

    public NguoiDungController(INguoiDungRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var user = await _repository.GetByIdAsync(int.Parse(userId));
        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> GetAll([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var (users, totalCount) = await _repository.SearchAsync(keyword, null, null, page, pageSize);
        return Ok(new { items = users, totalCount, page, pageSize });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] NguoiDung entity)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return NotFound();

        existing.ho_ten = entity.ho_ten;
        existing.email = entity.email;
        existing.so_dien_thoai = entity.so_dien_thoai;
        existing.dia_chi = entity.dia_chi;
        existing.vai_tro = entity.vai_tro;
        existing.trang_thai = entity.trang_thai;

        if (!string.IsNullOrEmpty(entity.mat_khau))
        {
            existing.mat_khau = entity.mat_khau;
        }

        await _repository.UpdateAsync(existing);
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null) return NotFound();

        await _repository.DeleteAsync(user);
        return Ok(new { message = "Đã xóa" });
    }
}