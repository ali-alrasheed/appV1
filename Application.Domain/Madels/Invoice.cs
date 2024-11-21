using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        /*
         List<Product>
         List<Store>
         */
        public decimal TotalAmount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime IssuedDate { get; set; }
        public bool IsActive { get; set; }
        public Invoice()
        {
            Id = Guid.NewGuid();
        }

    }
    public enum PaymentStatus
    {
        Pending,
        Watting
    }
}
