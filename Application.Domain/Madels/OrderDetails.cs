using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class OrderDetails
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public bool IsActive { get; set; } = true;

        public Guid OrderId { get; set; }
        public Order? Order { get; set; }


        public OrderDetails()
        {
            Id = Guid.NewGuid();
        }
    }
}
