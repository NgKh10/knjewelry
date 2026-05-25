using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Jewelry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HinhAnhSanPhamController : ControllerBase
{
    private readonly IHinhAnhSanPhamService _service;
    private readonly ISanPhamRepository _sanPhamRepository;

    public HinhAnhSanPhamController(IHinhAnhSanPhamService service, ISanPhamRepository sanPhamRepository)
    {
        _service = service;
        _sanPhamRepository = sanPhamRepository;
    }

    /// <summary>GET /api/hinhanhsanpham?idSanPham=&laChinhFilter=&tenSanPham=&page=&pageSize=&sortOrder=</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? idSanPham,
        [FromQuery] bool? laChinhFilter,
        [FromQuery] string? tenSanPham,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortOrder = null)
    {
        var (items, totalCount) = await _service.SearchAsync(idSanPham, laChinhFilter, tenSanPham, page, pageSize, sortOrder);

        return Ok(new
        {
            items,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            page,
            pageSize
        });
    }

    /// <summary>GET /api/hinhanhsanpham/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound(new { message = "Không tìm thấy hình ảnh!" });
        return Ok(item);
    }

    /// <summary>GET /api/hinhanhsanpham/sanpham/{idSanPham}</summary>
    [HttpGet("sanpham/{idSanPham:int}")]
    public async Task<IActionResult> GetBySanPham(int idSanPham)
    {
        var items = await _service.GetBySanPhamAsync(idSanPham);
        return Ok(items);
    }

    /// <summary>POST /api/hinhanhsanpham</summary>
    [HttpPost]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Create([FromBody] HinhAnhSanPham entity)
    {
        var sanPham = await _sanPhamRepository.GetByIdAsync(entity.id_san_pham);
        if (sanPham == null)
            return BadRequest(new { message = "Sản phẩm không tồn tại!" });

        try
        {
            var created = await _service.CreateAsync(entity);
            return Ok(new { created.id_hinh_anh, message = "Thêm hình ảnh thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>PUT /api/hinhanhsanpham/{id}</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] HinhAnhSanPham entity)
    {
        try
        {
            await _service.UpdateAsync(id, entity);
            return Ok(new { message = "Cập nhật hình ảnh thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>PUT /api/hinhanhsanpham/{id}/setmain</summary>
    [HttpPut("{id:int}/setmain")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> SetMain(int id)
    {
        try
        {
            await _service.SetMainAsync(id);
            return Ok(new { message = "Đã đặt làm ảnh chính!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>DELETE /api/hinhanhsanpham/{id}</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Xóa hình ảnh thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
