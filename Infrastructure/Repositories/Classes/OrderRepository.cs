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
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Classes
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext context;
        private readonly IDeliveryRepository deliveryRepository;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(AppDbContext context, IDeliveryRepository deliveryRepository, ILogger<OrderRepository> logger) : base(context)
        {
            this.context = context;
            this.deliveryRepository = deliveryRepository;
            _logger = logger;
        }

        public async Task<Order> AddAsync(OrderForCreate entity )
        {
            try
            {
                if (entity == null)
                {
                    _logger.LogWarning("Attempted to add a null OrderForCreate entity.");
                    return null;
                }

                var newOrder = new Order
                {
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    Status = entity.Status,
                    TotalPrice = entity.TotalPrice,
                    UserId = entity.UserId
                };
                await context.Orders.AddAsync(newOrder);

                var invoice = new Invoice
                {
                    Order = newOrder,
                    OrderId = newOrder.Id,
                    IsActive = true,
                    IssuedDate = DateTime.UtcNow,
                    Status = PaymentStatus.Watting,
                    TotalAmount = newOrder.TotalPrice,// بالاضافة الى اجرة التوصيل
                };
                await context.Invoices.AddAsync(invoice);
                await SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} created successfully.", newOrder.Id);
                return newOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new order.");
                throw;
            }
        }

        public async Task<(IList<Order>, PaginationMetaData)> GetAllAsync(int pageNumber, int pageSize, bool isInclude)
        {
            try
            {
                var query = context.Orders
                    .Where(o => o.IsActive)
                    .AsQueryable();

                var totalItemCount = await query.CountAsync();
                var paginationMetaData = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

                if (isInclude)
                {
                    query = query.Include(o => o.OrderDetails);
                }

                var orders = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {OrderCount} orders for page {PageNumber}.", orders.Count, pageNumber);

                return (orders, paginationMetaData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving orders.");
                throw;
            }
        }
    }
}
