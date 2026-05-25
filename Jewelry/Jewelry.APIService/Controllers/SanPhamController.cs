using Jewelry.Entities;
using Jewelry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SanPhamController : ControllerBase
{
    private readonly ISanPhamService _service;

    public SanPhamController(ISanPhamService service)
    {
        _service = service;
    }

    /// <summary>
    /// GET /api/sanpham?keyword=&loaiId=&giaTu=&giaDen=&page=&pageSize=
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] int? loaiId,
        [FromQuery] decimal? giaTu,
        [FromQuery] decimal? giaDen,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string? sortOrder = null)
    {
        var (products, totalCount) = await _service.SearchAsync(
            keyword, loaiId, giaTu, giaDen, page, pageSize, sortOrder);

        return Ok(new
        {
            items = products,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            page,
            pageSize
        });
    }

    /// <summary>
    /// GET /api/sanpham/{id}
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound(new { message = "Không tìm thấy sản phẩm!" });
        return Ok(item);
    }

    /// <summary>
    /// GET /api/sanpham/loai/{loaiId}
    /// </summary>
    [HttpGet("loai/{loaiId:int}")]
    public async Task<IActionResult> GetByLoai(int loaiId)
    {
        var (products, _) = await _service.SearchAsync(null, loaiId, null, null, 1, 100);
        return Ok(products);
    }

    /// <summary>
    /// POST /api/sanpham
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Create([FromBody] SanPham entity)
    {
        try
        {
            var created = await _service.CreateAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id = created.id_san_pham }, created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// PUT /api/sanpham/{id}
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] SanPham entity)
    {
        try
        {
            entity.id_san_pham = id;
            await _service.UpdateAsync(entity);
            return Ok(new { message = "Cập nhật thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// DELETE /api/sanpham/{id}
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Đã ẩn sản phẩm!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}