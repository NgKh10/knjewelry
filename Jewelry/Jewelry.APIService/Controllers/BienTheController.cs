using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Jewelry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BienTheController : ControllerBase
{
    private readonly IBienTheService _service;
    private readonly ISanPhamRepository _sanPhamRepository;

    public BienTheController(IBienTheService service, ISanPhamRepository sanPhamRepository)
    {
        _service = service;
        _sanPhamRepository = sanPhamRepository;
    }

    /// <summary>GET /api/bienthe?tenSanPham=&kichCo=&mauSac=&page=&pageSize=&sortOrder=</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? tenSanPham,
        [FromQuery] string? kichCo,
        [FromQuery] string? mauSac,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortOrder = null)
    {
        var (items, totalCount) = await _service.SearchAsync(tenSanPham, kichCo, mauSac, page, pageSize, sortOrder);

        return Ok(new
        {
            items,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            page,
            pageSize
        });
    }

    /// <summary>GET /api/bienthe/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound(new { message = "Không tìm thấy biến thể!" });
        return Ok(item);
    }

    /// <summary>GET /api/bienthe/sanpham/{idSanPham}</summary>
    [HttpGet("sanpham/{idSanPham:int}")]
    public async Task<IActionResult> GetBySanPham(int idSanPham)
    {
        var items = await _service.GetBySanPhamAsync(idSanPham);
        return Ok(items);
    }

    /// <summary>POST /api/bienthe</summary>
    [HttpPost]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Create([FromBody] BienThe entity)
    {
        var sanPham = await _sanPhamRepository.GetByIdAsync(entity.id_san_pham);
        if (sanPham == null)
            return BadRequest(new { message = "Sản phẩm không tồn tại!" });

        try
        {
            var created = await _service.CreateAsync(entity);
            return Ok(new { created.id_bien_the, message = "Thêm biến thể thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>PUT /api/bienthe/{id}</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] BienThe entity)
    {
        try
        {
            await _service.UpdateAsync(id, entity);
            return Ok(new { message = "Cập nhật biến thể thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>DELETE /api/bienthe/{id}</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Xóa biến thể thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
