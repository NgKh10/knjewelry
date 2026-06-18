using Jewelry.Repository;
using Microsoft.EntityFrameworkCore;

namespace Jewelry.Services
{
    public class DiscountExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DiscountExpiryService> _logger;

        public DiscountExpiryService(IServiceScopeFactory scopeFactory, ILogger<DiscountExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Chạy ngay lần đầu khi khởi động
            await DeactivateExpiredDiscounts();

            // Sau đó lặp mỗi 1 giờ
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                    await DeactivateExpiredDiscounts();
            }
        }

        private async Task DeactivateExpiredDiscounts()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTime.Now;
                var expired = await db.MaGiamGias
                    .Where(m => m.trang_thai == 1 && m.ngay_ket_thuc.HasValue && m.ngay_ket_thuc < now)
                    .ToListAsync();

                if (expired.Count > 0)
                {
                    foreach (var m in expired)
                        m.trang_thai = 0;

                    await db.SaveChangesAsync();
                    _logger.LogInformation("DiscountExpiryService: đã ngừng {Count} mã hết hạn", expired.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DiscountExpiryService: lỗi khi cập nhật mã hết hạn");
            }
        }
    }
}
