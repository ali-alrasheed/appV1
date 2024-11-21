using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Madels
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime DateTime { get; set; }
        public Payment()
        {
            Id = Guid.NewGuid();
        }
    }
    public enum PaymentType
    {
        Cash , Card 
    }
}
