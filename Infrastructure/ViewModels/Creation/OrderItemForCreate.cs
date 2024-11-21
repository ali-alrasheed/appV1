using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModels.Creation
{
    public class OrderItemForCreate
    {
       public int Quantity { get; set; }
       public Guid ProductId { get; set; }

    }
}
