using Application.Domain.Madels;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.ViewModels.Creation;
using Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Classes
{
    public class CategoriesRepository : GenericRepository<Category>, ICategoriesRepository
    {
        private readonly AppDbContext context;

        public CategoriesRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Category> AddAsync(CategoryForCreate entity)
        {
            if (entity is null)
                return null;
            var newCategory = new Category()
            {
                Name = entity.Name,
                IsActive = true,
            };
            context.Categories.Add(newCategory);
            await context.SaveChangesAsync();
            return newCategory;
        }

        public async Task<(IList<Category>, PaginationMetaData)> GetAllAsync(int pageNumber, int pageSize, bool isInclude)
        {
            var query = context.Categories.Where(c => c.IsActive).AsQueryable();

            var totalItemCount = await query.CountAsync();
            var paginationdata = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            if (isInclude)
                query = query.Include(c => c.Products);

            var all = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (all, paginationdata);
        }
    }

}
