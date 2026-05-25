using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoaiSanPhamController : ControllerBase
{
    private readonly ILoaiSanPhamRepository _repository;

    public LoaiSanPhamController(ILoaiSanPhamRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// GET /api/loaisanpham?keyword=&idLoaiCha=&page=&pageSize=
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] int? idLoaiCha,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortOrder = null)
    {
        var (items, totalCount) = await _repository.SearchAsync(keyword, idLoaiCha, page, pageSize, sortOrder);

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
    /// GET /api/loaisanpham/{id}
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return NotFound(new { message = "Không tìm thấy!" });
        return Ok(item);
    }

    /// <summary>
    /// POST /api/loaisanpham
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Create([FromBody] LoaiSanPham entity)
    {
        var exists = await _repository.GetByNameAsync(entity.ten_loai);
        if (exists != null)
            return BadRequest(new { message = "Tên danh mục đã tồn tại!" });

        await _repository.AddAsync(entity);
        return Ok(new { entity.id_loai_sp, entity.ten_loai, message = "Thêm thành công!" });
    }

    /// <summary>
    /// PUT /api/loaisanpham/{id}
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] LoaiSanPham entity)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Không tìm thấy!" });

        var nameExists = await _repository.GetByNameAsync(entity.ten_loai);
        if (nameExists != null && nameExists.id_loai_sp != id)
            return BadRequest(new { message = "Tên danh mục đã tồn tại!" });

        existing.ten_loai = entity.ten_loai;
        existing.id_loai_cha = entity.id_loai_cha;
        existing.thu_tu = entity.thu_tu;
        existing.mo_ta = entity.mo_ta;

        await _repository.UpdateAsync(existing);
        return Ok(new { message = "Cập nhật thành công!" });
    }

    /// <summary>
    /// DELETE /api/loaisanpham/{id}
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
            return NotFound(new { message = "Không tìm thấy!" });

        if (await _repository.HasChildrenAsync(id))
            return BadRequest(new { message = "Không thể xóa: Đang có danh mục con!" });

        if (await _repository.HasProductsAsync(id))
            return BadRequest(new { message = "Không thể xóa: Đang có sản phẩm!" });

        await _repository.DeleteAsync(item);
        return Ok(new { message = "Xóa thành công!" });
    }
}