using Application.Domain.Madels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModels.Creation
{
    public class DeliveryForCreate
    {
        public Guid OrderId { get; set; }
        public Guid DriverId { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
    }
}
