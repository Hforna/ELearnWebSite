using Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Order
{
    public class OrderShortResponse
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public CurrencyEnum Currency { get; set; }
        public TransactionStatusEnum Status { get; set; }
    }
}
