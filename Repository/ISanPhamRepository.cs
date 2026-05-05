using knjewelry.Models.Entities;

namespace knjewelry.Repository
{
    public interface ISanPhamRepository : IGenericRepository<SanPham>
    {
        Task<SanPham> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<SanPham>> GetNewProductsAsync(int take);
        Task<IEnumerable<SanPham>> GetHotProductsAsync(int take);
        Task<IEnumerable<SanPham>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<SanPham>> SearchProductsAsync(string keyword, int? categoryId);
        Task<int> GetStockAsync(int productId, int? variantId);
    }
}