using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jewelry.Services
{
    public class ChatLieuService : IChatLieuService
    {
        private readonly IChatLieuRepository _repository;

        public ChatLieuService(IChatLieuRepository repository)
        {
            _repository = repository;
        }

        public async Task<(List<ChatLieu> Items, int TotalCount)> SearchAsync(
            string? keyword, string? doTinhKhiet, int page, int pageSize, string? sortOrder = null)
        {
            return await _repository.SearchAsync(keyword, doTinhKhiet, page, pageSize, sortOrder);
        }

        public async Task<List<ChatLieu>> GetAllAsync()
        {
            var (items, _) = await _repository.SearchAsync(null, null, 1, int.MaxValue);
            return items;
        }

        public async Task<ChatLieu?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ChatLieu?> GetByNameAsync(string name)
        {
            return await _repository.GetByNameAsync(name);
        }

        public async Task<ChatLieu> CreateAsync(ChatLieu chatLieu)
        {
            var exists = await _repository.GetByNameAsync(chatLieu.ten_chat_lieu);
            if (exists != null)
                throw new Exception("Tên chất liệu đã tồn tại!");

            return await _repository.AddAsync(chatLieu);
        }

        public async Task UpdateAsync(ChatLieu chatLieu)
        {
            var existing = await _repository.GetByIdAsync(chatLieu.id_chat_lieu);
            if (existing == null)
                throw new Exception("Không tìm thấy chất liệu!");

            var duplicate = await _repository.GetByNameAsync(chatLieu.ten_chat_lieu);
            if (duplicate != null && duplicate.id_chat_lieu != chatLieu.id_chat_lieu)
                throw new Exception("Tên chất liệu đã tồn tại!");

            existing.ten_chat_lieu = chatLieu.ten_chat_lieu;
            existing.do_tinh_khiet = chatLieu.do_tinh_khiet;
            existing.mo_ta = chatLieu.mo_ta;

            await _repository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Không tìm thấy chất liệu!");

            if (await _repository.HasProductsAsync(id))
                throw new Exception("Không thể xóa: Chất liệu đang được sử dụng!");

            await _repository.DeleteAsync(existing);
        }
    }
}