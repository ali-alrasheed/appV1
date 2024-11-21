using Application.Domain.Madels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<List<Invoice>> GetInLastSevenDays();
        Task<List<Invoice>> GetInLastMonth();
        Task<List<Invoice>> GetInLastYear();
        Task<List<Invoice>> GetBetweenTowDate(DateTime startDate, DateTime endDate);
    }
}
