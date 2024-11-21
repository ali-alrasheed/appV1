using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public Guid StoreId { get; set; }
        public Store? Store { get; set; }
    }
}
