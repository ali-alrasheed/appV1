using Application.Domain.Madels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModels.Creation
{
    public class OrderForCreate
    {
        /*        public DateTime DateOrder { get; set; }
                public Guid UserId { get; set; }

                public List<OrderItemForCreate> Items { get; set; } = new List<OrderItemForCreate>();*/
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        /*public List<Guid> StoresId { get; set; } = new List<Guid>();*/
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemForCreate> Items { get; set; } = new List<OrderItemForCreate>();
    }
}
