using Application.Domain.Madels;
using Infrastructure.ViewModels;
using Infrastructure.ViewModels.Creation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IStoreRepository : IRepository<Store>
    {
        Task<(IList<Store>, PaginationMetaData)> GetAllAsync(int pageNumber, int pageSize, bool isInclude);
        Task<Store> AddAsync(StoreForCreate entity);
    }
}
