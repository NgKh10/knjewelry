using Jewelry.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jewelry.Services
{
    public interface IHoaDonService
    {
        Task<HoaDon> CreateAsync(HoaDon hoaDon);
        Task<List<HoaDon>> GetByUserAsync(int userId);
        Task<HoaDon?> GetByIdAsync(int id);
    }
}