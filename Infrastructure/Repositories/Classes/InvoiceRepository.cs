using Application.Domain.Madels;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Classes
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly AppDbContext context;

        public InvoiceRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<Invoice>> GetBetweenTowDate(DateTime startDate, DateTime endDate)
        {
            var invoices = await context.Invoices
                           .Where(i => i.IssuedDate >= startDate && i.IssuedDate <= endDate && i.IsActive)
                           .ToListAsync();
            return invoices;
        }

        public async Task<List<Invoice>> GetInLastMonth()
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime lastMonthDate = currentDate.AddMonths(-1);
            lastMonthDate = lastMonthDate.AddDays(-lastMonthDate.Day + 1);
            DateTime nextMonth = lastMonthDate.AddMonths(1);
            var invoices = await context.Invoices
                        .Where(i => i.IssuedDate >= lastMonthDate && i.IssuedDate < nextMonth && i.IsActive)
                        .ToListAsync();
            return invoices;
        }

        public async Task<List<Invoice>> GetInLastSevenDays()
        {
            DateTime currentDate = DateTime.UtcNow;
            DateTime dateOfLastSevenDays = currentDate.AddDays(-7);
            var invoices = await context.Invoices
                        .Where(i => i.IssuedDate >= dateOfLastSevenDays && i.IssuedDate < currentDate && i.IsActive)
                        .ToListAsync();
            return invoices;
        }

        public async Task<List<Invoice>> GetInLastYear()
        {
            DateTime currentDate = DateTime.UtcNow;

            DateTime lastYear = currentDate.AddYears(-1);
            lastYear = lastYear.AddMonths(-lastYear.Month + 1);
            lastYear = lastYear.AddDays(-lastYear.Day + 1);

            DateTime nextYear = lastYear.AddYears(1);

            var invoices = await context.Invoices
                   .Where(i => i.IssuedDate >= lastYear && i.IssuedDate < nextYear && i.IsActive)
                   .ToListAsync();
            return invoices;

        }
    }
}
