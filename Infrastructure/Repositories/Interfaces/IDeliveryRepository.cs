using Application.Domain.Madels;
using Infrastructure.ViewModels.Creation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IDeliveryRepository : IRepository<Delivery>
    {
        Task<Delivery> AddAsync(DeliveryForCreate entity);
        Task OrderDelivered(Guid OrderId, Guid InvoiceId);
        Task<IList<Delivery>> GetDeliveriesByStatusAsync(DeliveryStatus status);
        Task UpdateDeliveryStatus(Guid deliveryId, DeliveryStatus newStatus);

        // لربط توصيل مع سائق محدد 
        Task AssignDriverToDelivery(Guid deliveryId, Guid driverId);

        // استرجاع سجل توصيلات سائق محدد
        Task<IList<Delivery>> GetDeliveriesByDriverAsync(Guid driverId);

        // الغاء توصيل معين 
        Task CancelDelivery(Guid deliveryId);


    }
}
