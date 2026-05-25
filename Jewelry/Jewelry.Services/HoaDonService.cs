using Jewelry.Entities;
using Jewelry.Repository.EFCore;
using Jewelry.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jewelry.Services
{
    public class HoaDonService : IHoaDonService
    {
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly IRepository<ChiTietHoaDon> _chiTietRepository;
        private readonly ISanPhamRepository _sanPhamRepository;

        public HoaDonService(
            IHoaDonRepository hoaDonRepository,
            IRepository<ChiTietHoaDon> chiTietRepository,
            ISanPhamRepository sanPhamRepository)
        {
            _hoaDonRepository = hoaDonRepository;
            _chiTietRepository = chiTietRepository;
            _sanPhamRepository = sanPhamRepository;
        }

        public async Task<HoaDon> CreateAsync(HoaDon hoaDon)
        {
            hoaDon.thoi_gian_dat = DateTime.Now;
            hoaDon.trang_thai = "Chờ xác nhận";

            await _hoaDonRepository.AddAsync(hoaDon);
            return hoaDon;
        }

        public async Task<List<HoaDon>> GetByUserAsync(int userId)
        {
            return await _hoaDonRepository.GetByUserAsync(userId);
        }

        public async Task<HoaDon?> GetByIdAsync(int id)
        {
            var hoaDon = await _hoaDonRepository.GetByIdAsync(id);

            if (hoaDon != null)
            {
                var chiTiets = await _chiTietRepository.FindAsync(ct => ct.id_hoa_don == id);
                hoaDon.ChiTietHoaDons = chiTiets.ToList();

                foreach (var ct in hoaDon.ChiTietHoaDons)
                {
                    ct.SanPham = await _sanPhamRepository.GetByIdAsync(ct.id_san_pham);
                }
            }

            return hoaDon;
        }
    }
}