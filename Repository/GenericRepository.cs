using Microsoft.EntityFrameworkCore;
using knjewelry.Data;
using System.Linq.Expressions;

namespace knjewelry.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly TrangSucBacContext _context;
        protected readonly DbSet<T> _dbSet;

        /// <summary>
        /// Khởi tạo Generic Repository với DbContext.
        /// </summary>
        /// <param name="context">Đối tượng DbContext thao tác với cơ sở dữ liệu.</param>
        public GenericRepository(TrangSucBacContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Lấy một bản ghi theo khóa chính.
        /// </summary>
        /// <param name="id">Khóa chính của đối tượng.</param>
        /// <returns>Đối tượng tìm được hoặc null nếu không tồn tại.</returns>
        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Lấy toàn bộ dữ liệu của thực thể.
        /// </summary>
        /// <returns>Danh sách tất cả các bản ghi.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Tìm kiếm các bản ghi thỏa mãn điều kiện.
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện lọc.</param>
        /// <returns>Danh sách các bản ghi phù hợp.</returns>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Lấy một bản ghi duy nhất thỏa mãn điều kiện hoặc trả về null.
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện lọc.</param>
        /// <returns>Đối tượng tìm được hoặc null.</returns>
        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.SingleOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Thêm một đối tượng mới vào DbSet.
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm.</param>
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Thêm nhiều đối tượng vào DbSet.
        /// </summary>
        /// <param name="entities">Danh sách đối tượng cần thêm.</param>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// Cập nhật thông tin của một đối tượng.
        /// </summary>
        /// <param name="entity">Đối tượng cần cập nhật.</param>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Xóa một đối tượng khỏi DbSet.
        /// </summary>
        /// <param name="entity">Đối tượng cần xóa.</param>
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Xóa nhiều đối tượng khỏi DbSet.
        /// </summary>
        /// <param name="entities">Danh sách đối tượng cần xóa.</param>
        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <summary>
        /// Đếm số lượng bản ghi thỏa mãn điều kiện.
        /// Nếu không truyền điều kiện thì đếm toàn bộ bản ghi.
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện lọc.</param>
        /// <returns>Số lượng bản ghi.</returns>
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();
            return await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Kiểm tra xem có tồn tại bản ghi thỏa mãn điều kiện hay không.
        /// </summary>
        /// <param name="predicate">Biểu thức điều kiện kiểm tra.</param>
        /// <returns>True nếu tồn tại, ngược lại False.</returns>
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Lưu các thay đổi xuống cơ sở dữ liệu.
        /// </summary>
        /// <returns>Số bản ghi bị ảnh hưởng.</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}