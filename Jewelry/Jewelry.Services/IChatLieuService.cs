using Jewelry.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jewelry.Services
{
    public interface IChatLieuService
    {
         Task<(List<ChatLieu> Items, int TotalCount)> SearchAsync(
            string? keyword, string? doTinhKhiet, int page, int pageSize, string? sortOrder = null);
        Task<List<ChatLieu>> GetAllAsync();
        Task<ChatLieu?> GetByIdAsync(int id);
        Task<ChatLieu?> GetByNameAsync(string name);
        Task<ChatLieu> CreateAsync(ChatLieu chatLieu);
        Task UpdateAsync(ChatLieu chatLieu);
        Task DeleteAsync(int id);
    }
}