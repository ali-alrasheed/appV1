using Application.Domain.Madels;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Interfaces;
using Infrastructure.ViewModels.Creation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Classes
{
    public class DeliveryRepository : GenericRepository<Delivery>, IDeliveryRepository
    {
        private readonly AppDbContext context;

        public DeliveryRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }
        // اضافة توصيل جديد 
        public async Task<Delivery> AddAsync(DeliveryForCreate entity)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == entity.OrderId && o.IsActive);
            var driver = await context.Users.FirstOrDefaultAsync(u => u.Id == entity.DriverId);
            var newDelivery = new Delivery
            {
                DeliveryStatus = entity.DeliveryStatus,
                DriverId = entity.DriverId,
                IsActive = true,
                OrderId = entity.OrderId,
            };
            context.Deliveries.Add(newDelivery);
            await context.SaveChangesAsync();

            return newDelivery;
        }

        // اسناد توصيل بسائق معين 
        public async Task AssignDriverToDelivery(Guid deliveryId, Guid driverId)
        {
            var delivery = await context.Deliveries.FirstOrDefaultAsync(d => d.Id == deliveryId && d.IsActive);
            var driver = await context.Users.FirstOrDefaultAsync(u => u.Id == driverId);

            if (delivery != null && driver != null)
            {
                delivery.DriverId = driverId;
                context.Deliveries.Update(delivery);
                await context.SaveChangesAsync();
            }
        }

        // الغاء توصيل معين 
        public async Task CancelDelivery(Guid deliveryId)
        {
            var delivery = await context.Deliveries.FirstOrDefaultAsync(d => d.Id == deliveryId && d.IsActive);
            if (delivery != null)
            {
                delivery.IsActive = false;
                delivery.DeliveryStatus = DeliveryStatus.Canceled;
                context.Deliveries.Update(delivery);
                await context.SaveChangesAsync();
            }
        }
        
        // عرض توصيلات سائق معين 
        public async Task<IList<Delivery>> GetDeliveriesByDriverAsync(Guid driverId)
        {
            var deliveries = await context.Deliveries
                            .Where(d => d.DriverId == driverId && d.IsActive)
                            .Include(d => d.Order)
                            .ToListAsync();
            return deliveries;
        }
        
        // عرض التوصيلات حسب حالة التوصيل 
        public async Task<IList<Delivery>> GetDeliveriesByStatusAsync(DeliveryStatus status)
        {
            var deliveries = await context.Deliveries
                            .Where(d => d.DeliveryStatus == status && d.IsActive)
                            .Include(d => d.Driver)
                            .Include(d => d.Order)
                            .ToListAsync();
            return deliveries;
        }

        // التوصيل وصل للعميل 
        public async Task OrderDelivered(Guid OrderId , Guid InvoiceId)
        {
            var order = await context.Orders.FirstOrDefaultAsync(o => o.Id == OrderId && o.IsActive);
            order!.Status = Status.Success;

            var invoice = await context.Invoices.FirstOrDefaultAsync(i => i.Id == InvoiceId && i.IsActive);
            invoice!.Status = PaymentStatus.Pending;

            context.Invoices.Update(invoice);   
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        
        // تحديث حالة توصيل معين 
        public async Task UpdateDeliveryStatus(Guid deliveryId, DeliveryStatus newStatus)
        {
            var delivery = await context.Deliveries.FirstOrDefaultAsync(d => d.Id == deliveryId && d.IsActive);
            if (delivery != null)
            {
                delivery.DeliveryStatus = newStatus;
                context.Deliveries.Update(delivery);
                await context.SaveChangesAsync();
            }
        }
    }
}
