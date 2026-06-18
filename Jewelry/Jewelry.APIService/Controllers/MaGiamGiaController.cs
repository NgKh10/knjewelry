using Jewelry.Entities;
using Jewelry.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MaGiamGiaController : ControllerBase
{
    private readonly IMaGiamGiaService _service;

    public MaGiamGiaController(IMaGiamGiaService service)
    {
        _service = service;
    }

    /// <summary>GET /api/magiamgia?keyword=&loaiGiam=&trangThai=&page=&pageSize=&sortOrder=</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] string? loaiGiam,
        [FromQuery] byte? trangThai,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortOrder = null)
    {
        var (items, totalCount) = await _service.SearchAsync(keyword, loaiGiam, trangThai, page, pageSize, sortOrder);

        return Ok(new
        {
            items,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            page,
            pageSize
        });
    }

    /// <summary>GET /api/magiamgia/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound(new { message = "Không tìm thấy mã giảm giá!" });
        return Ok(item);
    }

    /// <summary>GET /api/magiamgia/check/{code}?orderTotal=</summary>
    [HttpGet("check/{code}")]
    public async Task<IActionResult> CheckCode(string code, [FromQuery] decimal orderTotal = 0)
    {
        var item = await _service.GetByCodeAsync(code);
        if (item == null)
            return NotFound(new { message = "Mã giảm giá không tồn tại!" });

        if (item.trang_thai != 1)
            return BadRequest(new { message = "Mã giảm giá không còn hiệu lực!" });

        if (item.ngay_bat_dau.HasValue && item.ngay_bat_dau > DateTime.Now)
            return BadRequest(new { message = "Mã giảm giá chưa đến ngày sử dụng!" });

        if (item.ngay_ket_thuc.HasValue && item.ngay_ket_thuc < DateTime.Now)
            return BadRequest(new { message = "Mã giảm giá đã hết hạn!" });

        if (item.so_luong.HasValue && item.da_su_dung >= item.so_luong)
            return BadRequest(new { message = "Mã giảm giá đã hết lượt sử dụng!" });

        if (orderTotal > 0 && orderTotal < item.don_hang_toi_thieu)
            return BadRequest(new { message = $"Đơn hàng tối thiểu {item.don_hang_toi_thieu:N0}đ để dùng mã này!" });

        decimal discountAmount = 0;
        if (item.loai_giam == "%" || item.loai_giam == "phan_tram")
        {
            discountAmount = orderTotal * item.gia_tri / 100;
            if (item.giam_toi_da.HasValue && discountAmount > item.giam_toi_da)
                discountAmount = item.giam_toi_da.Value;
        }
        else
        {
            discountAmount = item.gia_tri;
        }

        return Ok(new { valid = true, item, discountAmount });
    }

    /// <summary>POST /api/magiamgia</summary>
    [HttpPost]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Create([FromBody] MaGiamGia entity)
    {
        try
        {
            var created = await _service.CreateAsync(entity);
            return Ok(new { created.id_ma_giam_gia, created.ma_code, message = "Tạo mã giảm giá thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>PUT /api/magiamgia/{id}</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Update(int id, [FromBody] MaGiamGia entity)
    {
        try
        {
            await _service.UpdateAsync(id, entity);
            return Ok(new { message = "Cập nhật mã giảm giá thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>DELETE /api/magiamgia/{id}</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Xóa mã giảm giá thành công!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
