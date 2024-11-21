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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger) : base(context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Product> AddAsync(ProductForCreate entity)
        {
            try
            {
                if (entity == null)
                {
                    _logger.LogWarning("Attempted to add a null ProductForCreate entity.");
                    return null;
                }

                var newProduct = new Product
                {
                    Name = entity.Name,
                    CategoryId = entity.CategoryId,
                    Description = entity.Description,
                    IsActive = true,
                    Price = entity.Price,
                    StoreId = entity.StoreId,
                    Quantity = entity.Quantity,
                    
                };

                await _context.Products.AddAsync(newProduct);
                await SaveChangesAsync();

                _logger.LogInformation("Product {ProductName} created successfully with ID: {ProductId}.", newProduct.Name, newProduct.Id);
                return newProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new product.");
                throw;
            }
        }

        public async Task<(IList<Product>, PaginationMetaData)> GetAllAsync(int pageNumber, int pageSize, bool isInclude)
        {
            try
            {
                var query = _context.Products
                    .Where(p => p.IsActive)
                    .AsQueryable();

                var totalItemCount = await query.CountAsync();
                var paginationMetaData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

                if (isInclude)
                {
                    // Example of eager loading related entities:
                    query = query.Include(p => p.Category); // Include related Category if needed
                }

                var products = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {ProductCount} products for page {PageNumber}.", products.Count, pageNumber);

                return (products, paginationMetaData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products.");
                throw;
            }
        }
    }
}
