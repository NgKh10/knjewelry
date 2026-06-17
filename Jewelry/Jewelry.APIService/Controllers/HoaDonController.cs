using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Jewelry.APIService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HoaDonController : ControllerBase
{
    private readonly IHoaDonRepository _repository;
    private readonly IChiTietHoaDonRepository _chiTietRepository;

    // Trạng thái hợp lệ và các chuyển đổi được phép
    private static readonly Dictionary<string, List<string>> ValidTransitions = new()
    {
        ["Chờ xác nhận"] = new List<string> { "Đã xác nhận", "Hủy" },
        ["Đã xác nhận"] = new List<string> { "Đang giao", "Hủy" },
        ["Đang giao"] = new List<string> { "Hoàn thành", "Hủy" },
        ["Hoàn thành"] = new List<string>(),
        ["Hủy"] = new List<string>()
    };

    public HoaDonController(IHoaDonRepository repository, IChiTietHoaDonRepository chiTietRepository)
    {
        _repository = repository;
        _chiTietRepository = chiTietRepository;
    }

    /// <summary>GET /api/hoadon - Lấy đơn hàng của user hiện tại</summary>
    [HttpGet]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] string? trangThai,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = int.Parse(User.FindFirst("id")?.Value ?? "0");
        var (items, totalCount) = await _repository.SearchAsync(null, trangThai, null, null, userId, page, pageSize);

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
    /// GET /api/hoadon/all - Admin: tìm kiếm tất cả đơn hàng
    /// Tìm theo: keyword (mã HĐ/tên/email/SĐT), trang_thai, tuNgay, denNgay
    /// </summary>
    [HttpGet("all")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] string? trangThai,
        [FromQuery] DateTime? tuNgay,
        [FromQuery] DateTime? denNgay,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortOrder = null)
    {
        var (items, totalCount) = await _repository.SearchAsync(keyword, trangThai, tuNgay, denNgay, null, page, pageSize, sortOrder);

        return Ok(new
        {
            items,
            totalCount,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            page,
            pageSize
        });
    }

    /// <summary>GET /api/hoadon/{id}</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _repository.GetWithDetailsAsync(id);
        if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng!" });

        var userId = User.FindFirst("id")?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        // Khách hàng chỉ xem được đơn của mình
        if (role != "quan_tri" && order.id_nguoi_dung?.ToString() != userId)
            return Forbid();

        return Ok(order);
    }

    /// <summary>POST /api/hoadon - Tạo đơn hàng mới</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HoaDon entity)
    {
        if (string.IsNullOrWhiteSpace(entity.ho_ten))
            return BadRequest(new { message = "Họ tên không được để trống!" });

        if (string.IsNullOrWhiteSpace(entity.so_dien_thoai))
            return BadRequest(new { message = "Số điện thoại không được để trống!" });

        if (string.IsNullOrWhiteSpace(entity.dia_chi_cu_the))
            return BadRequest(new { message = "Địa chỉ giao hàng không được để trống!" });

        var userId = User.FindFirst("id")?.Value;
        entity.id_nguoi_dung = int.Parse(userId ?? "0");
        entity.thoi_gian_dat = DateTime.Now;
        entity.trang_thai = "Chờ xác nhận";
        entity.ma_hoa_don = $"HD{DateTime.Now:yyyyMMddHHmmss}";

        var created = await _repository.AddAsync(entity);
        return Ok(new { created.id_hoa_don, created.ma_hoa_don, message = "Đặt hàng thành công!" });
    }

    /// <summary>PUT /api/hoadon/bulk-status - Admin: cập nhật hàng loạt trạng thái đơn hàng</summary>
    [HttpPut("bulk-status")]
    [Authorize(Roles = "quan_tri")]
    public async Task<IActionResult> BulkUpdateStatus([FromBody] BulkUpdateStatusRequest request)
    {
        if (request.ids == null || request.ids.Count == 0)
            return BadRequest(new { message = "Danh sách đơn hàng không được để trống!" });

        int success = 0, failed = 0;
        var errors = new List<string>();

        foreach (var id in request.ids)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null) { failed++; errors.Add($"Đơn #{id}: Không tìm thấy"); continue; }

            if (!ValidTransitions.TryGetValue(order.trang_thai, out var allowedNext) || !allowedNext.Contains(request.trang_thai))
            {
                failed++;
                errors.Add($"Đơn #{id}: Không thể chuyển từ '{order.trang_thai}' sang '{request.trang_thai}'");
                continue;
            }

            order.trang_thai = request.trang_thai;
            if (request.trang_thai == "Hoàn thành")
                order.thoi_gian_giao_tt = DateTime.Now;

            await _repository.UpdateAsync(order);
            success++;
        }

        return Ok(new
        {
            message = $"Đã cập nhật {success} đơn hàng" + (failed > 0 ? $", {failed} đơn thất bại" : ""),
            success,
            failed,
            errors
        });
    }

    /// <summary>
    /// PUT /api/hoadon/{id}/status - Cập nhật trạng thái đơn hàng
    /// Chỉ admin mới chuyển trạng thái, ngoại trừ khách hàng có thể hủy đơn "Chờ xác nhận"
    /// </summary>
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        var order = await _repository.GetByIdAsync(id);
        if (order == null)
            return NotFound(new { message = "Không tìm thấy đơn hàng!" });

        var userId = User.FindFirst("id")?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        // Khách hàng chỉ được hủy đơn của mình khi đang ở trạng thái "Chờ xác nhận"
        if (role != "quan_tri")
        {
            if (order.id_nguoi_dung?.ToString() != userId)
                return Forbid();

            if (request.trang_thai != "Hủy")
                return Forbid();

            if (order.trang_thai != "Chờ xác nhận")
                return BadRequest(new { message = "Chỉ có thể hủy đơn hàng đang ở trạng thái 'Chờ xác nhận'!" });
        }

        // Kiểm tra chuyển trạng thái hợp lệ
        if (!ValidTransitions.ContainsKey(order.trang_thai))
            return BadRequest(new { message = $"Trạng thái hiện tại '{order.trang_thai}' không hợp lệ!" });

        var allowedNext = ValidTransitions[order.trang_thai];
        if (!allowedNext.Contains(request.trang_thai))
            return BadRequest(new
            {
                message = $"Không thể chuyển từ '{order.trang_thai}' sang '{request.trang_thai}'!",
                allowedTransitions = allowedNext
            });

        var oldStatus = order.trang_thai;
        order.trang_thai = request.trang_thai;

        if (request.trang_thai == "Hoàn thành")
            order.thoi_gian_giao_tt = DateTime.Now;

        if (!string.IsNullOrEmpty(request.ghi_chu))
            order.ghi_chu = request.ghi_chu;

        await _repository.UpdateAsync(order);
        return Ok(new
        {
            message = $"Đã chuyển trạng thái từ '{oldStatus}' sang '{request.trang_thai}'!",
            trang_thai = order.trang_thai
        });
    }
}

public class UpdateStatusRequest
{
    public string trang_thai { get; set; } = "";
    public string? ghi_chu { get; set; }
}

public class BulkUpdateStatusRequest
{
    public List<int> ids { get; set; } = new();
    public string trang_thai { get; set; } = "";
}
