using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Jewelry.Entities;
using Jewelry.Repository.EFCore;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatLieuController : ControllerBase
{
    private readonly IChatLieuRepository _repository;

    public ChatLieuController(IChatLieuRepository repository)
    {
        _repository = repository;
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
        var (items, totalCount) = await _repository.SearchAsync(keyword, doTinhKhiet, page, pageSize, sortOrder);

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
        var item = await _repository.GetByIdAsync(id);
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

        var exists = await _repository.GetByNameAsync(entity.ten_chat_lieu);
        if (exists != null)
            return BadRequest(new { message = "Tên chất liệu đã tồn tại!" });

        await _repository.AddAsync(entity);
        return Ok(new { entity.id_chat_lieu, entity.ten_chat_lieu, message = "Thêm thành công!" });
    }

    /// <summary>
    /// PUT /api/chatlieu/{id}
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] ChatLieu entity)
    {
        if (string.IsNullOrWhiteSpace(entity.ten_chat_lieu))
            return BadRequest(new { message = "Tên chất liệu không được để trống!" });

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Không tìm thấy chất liệu!" });

        var duplicate = await _repository.GetByNameAsync(entity.ten_chat_lieu);
        if (duplicate != null && duplicate.id_chat_lieu != id)
            return BadRequest(new { message = "Tên chất liệu đã tồn tại!" });

        existing.ten_chat_lieu = entity.ten_chat_lieu;
        existing.do_tinh_khiet = entity.do_tinh_khiet;
        existing.mo_ta = entity.mo_ta;

        await _repository.UpdateAsync(existing);
        return Ok(new { message = "Cập nhật thành công!" });
    }

    /// <summary>
    /// DELETE /api/chatlieu/{id}
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
            return NotFound(new { message = "Không tìm thấy chất liệu!" });

        if (await _repository.HasProductsAsync(id))
            return BadRequest(new { message = "Không thể xóa: Chất liệu đang được sử dụng!" });

        await _repository.DeleteAsync(item);
        return Ok(new { message = "Xóa thành công!" });
    }
}