using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class User : IdentityUser<Guid>
    {

        public string? Name { get; set; }
        public string? Address { get; set; }    
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
