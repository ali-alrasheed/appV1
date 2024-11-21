using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        /*Task<T> AddAsync(T entity);*/
        T Update(T entity);
        Task<T?> GetAsync(Guid id, bool isInclude = false);
        Task<T?> DeleteAsync(Guid id);
        // Task<IList<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10 ,bool isInclude = false);
        Task<int> SaveChangesAsync();
        Task<bool> ExistsAsync(Guid id);
        //void SaveChanges();
    }
}
