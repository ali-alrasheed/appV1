using Application.Domain.Madels;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.ViewModels;
using Infrastructure.ViewModels.Creation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Classes
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StoreRepository> _logger;

        public StoreRepository(AppDbContext context, ILogger<StoreRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }
        // من اجل اضافة متجر جديد
        public async Task<Store> AddAsync(StoreForCreate entity)
        {
            try
            {
                if (entity == null)
                {
                    _logger.LogWarning("Attempted to add a null StoreForCreate entity.");
                    throw new Exception("Store not Created");
                }

                var newStore = new Store()
                {
                    Location = entity.Location,
                    Name = entity.Name,
                    IsActive = true,
                    OwnerId = entity.OwnerId,
                    Status = entity.Status,
                };

                await _context.Stores.AddAsync(newStore);
                await SaveChangesAsync();

                _logger.LogInformation("Store {StoreName} created successfully with ID: {StoreId}.", newStore.Name, newStore.Id);
                return newStore;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new store.");
                throw;
            }
        }

        // عرض كل المتاجر
        public async Task<(IList<Store>, PaginationMetaData)> GetAllAsync(int pageNumber, int pageSize, bool isInclude)
        {
            try
            {
                var query = _context.Stores
                    .Where(s => s.IsActive)
                    .AsQueryable();

                var totalItemCount = await query.CountAsync();
                var paginationMetaData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

                if (isInclude)
                {
                    query = query.Include(s => s.Products);
                }

                var stores = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {StoreCount} stores for page {PageNumber}.", stores.Count, pageNumber);

                return (stores, paginationMetaData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving stores.");
                throw;
            }
        }
    }
}
