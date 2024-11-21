using Application.Domain.Madels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart> CreateOrUpdate(Guid? carteId, Guid userId, Guid saplingId, int Quantity = 1);
        Task<Cart> GetCartWithDetails(Guid cartId);
        Task<Order> ConfirmOrder(Guid cartId, Guid userId, int discount = 0);
        Task DeleteCartItem(Guid cartItemId);
    }
}
