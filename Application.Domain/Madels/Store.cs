using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class Store
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public StatusOfStore Status { get; set; }
        public required string Location { get; set; }
        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }
        public bool IsActive { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Order> Orders { get; set; } = new List<Order>();
    }
    public enum StatusOfStore
    {
        Open , Close
    }
}
