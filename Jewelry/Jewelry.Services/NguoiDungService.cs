using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using System.Threading.Tasks;

namespace Jewelry.Services
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly INguoiDungRepository _nguoiDungRepository;

        public NguoiDungService(INguoiDungRepository nguoiDungRepository)
        {
            _nguoiDungRepository = nguoiDungRepository;
        }

        public async Task<NguoiDung?> LoginAsync(string username, string password)
        {
            return await _nguoiDungRepository.LoginAsync(username, password);
        }
    }
}