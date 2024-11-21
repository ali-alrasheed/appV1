using Infrastructure.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext context;

        public GenericRepository(AppDbContext context)
        {
            this.context = context;
        }
        // delete an entity
        public virtual async Task<T?> DeleteAsync(Guid id)
        {
            var entity = await context.FindAsync<T>(id);
            if (entity == null)
                return null;

            if (typeof(T).GetProperty("IsActive") != null)
            {
                typeof(T).GetProperty("IsActive")?.SetValue(entity, false);
            }

            context.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await context.Set<T>().FindAsync(id) != null;
        }

        public virtual async Task<T?> GetAsync(Guid id, bool isInclude = false)
        {
            return await context.FindAsync<T>(id);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public T Update(T entity)
        {
            return context.Update(entity).Entity;
        }
    }
}
