using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class Cart
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal TotalPrice => CartItems.Sum(x => x.Price);   
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public Cart()
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.UtcNow;
        }
    }
}
