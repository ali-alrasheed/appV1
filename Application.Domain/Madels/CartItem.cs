using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public int Quantity {  get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
        public bool IsActive { get; set; }
        public Guid CartId { get; set; }
        public Cart? Cart { get; set; }
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public CartItem()
        {
            Id = Guid.NewGuid();
        }
    }
}
