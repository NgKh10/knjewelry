using Jewelry.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jewelry.Services
{
    public interface INguoiDungService
    {
        Task<NguoiDung?> LoginAsync(string username, string password);
    }
}