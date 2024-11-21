using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class Delivery
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        public Guid DriverId { get; set; }
        public User? Driver {  get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public bool IsActive { get; set; }
        public Delivery()
        {
            Id = Guid.NewGuid();
        }

    }
    public enum DeliveryStatus
    {
        Completed,
        InWay,
        Canceled
    }
}
