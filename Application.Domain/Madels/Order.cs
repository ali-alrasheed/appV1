using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    /*    public Guid StoreId { get; set; }
        public Store? Store { get; set; }*/
        /*public List<Guid> StoresId { get; set; } = new List<Guid>();*/
        public Guid DeliveryId { get; set; }
        public Delivery? Delivery { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
    public enum Status
    {
        Success,Faild ,InWay
    }
}
