using Application.Domain.Madels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModels.Creation
{
    public class StoreForCreate
    {
        public required string Name { get; set; }
        public StatusOfStore Status { get; set; }
        public required string Location { get; set; }
        public Guid OwnerId { get; set; }
    }
}
