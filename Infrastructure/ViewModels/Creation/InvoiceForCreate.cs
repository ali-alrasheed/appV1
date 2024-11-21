using Application.Domain.Madels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModels.Creation
{
    public class InvoiceForCreate
    {
        public Guid OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime IssuedDate { get; set; }
    }
}
