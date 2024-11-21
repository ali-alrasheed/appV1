using Application.Domain.Madels;
using Infrastructure.ViewModels.Creation;
using Infrastructure.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<(IList<Order>, PaginationMetaData)> GetAllAsync(int pageNumber, int pageSize, bool isInclude);
        Task<Order> AddAsync(OrderForCreate entity);
    }
}