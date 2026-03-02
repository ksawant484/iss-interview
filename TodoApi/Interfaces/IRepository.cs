namespace TodoApi.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> AddAsync(T resource);
        
        Task<T?> GetByIdAsync(int id);
        
        Task<IEnumerable<T>> GetAllAsync();

        Task<bool> DeleteAsync(int id);

        Task<T> UpdateAsync(T resource);

        Task<bool> ExistsAsync(int id);
    }
}
