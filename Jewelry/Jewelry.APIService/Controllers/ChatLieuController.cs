using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Jewelry.Services;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatLieuController : ControllerBase
{
    private readonly IChatLieuService _service;

    public ChatLieuController(IChatLieuService service)
    {
        _service = service;
    }

    /// <summary>
    /// GET /api/chatlieu?keyword=&doTinhKhiet=&page=&pageSize=
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] string? doTinhKhiet,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortOrder = null)
    {
        var (items, totalCount) = await _service.SearchAsync(keyword, doTinhKhiet, page, pageSize, sortOrder);

        return Ok(new
        {
            items,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            page,
            pageSize
        });
    }

    /// <summary>
    /// GET /api/chatlieu/{id}
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound(new { message = "Không tìm thấy chất liệu!" });
        return Ok(item);
    }

    /// <summary>
    /// POST /api/chatlieu
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Create([FromBody] ChatLieu entity)
    {
        if (string.IsNullOrWhiteSpace(entity.ten_chat_lieu))
            return BadRequest(new { message = "Tên chất liệu không được để trống!" });

        await _service.CreateAsync(entity);
        return Ok(new { entity.id_chat_lieu, entity.ten_chat_lieu, message = "Thêm thành công!" });
    }

    /// <summary>
    /// PUT /api/chatlieu/{id}
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] ChatLieu entity)
    {
        entity.id_chat_lieu = id; // Đảm bảo ID đồng bộ

        try
        {
            await _service.UpdateAsync(entity);
            return Ok(new { message = "Cập nhật thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/chatlieu/{id}
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Xóa thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}