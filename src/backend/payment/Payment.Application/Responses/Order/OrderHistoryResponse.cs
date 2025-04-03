using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Responses.Order
{
    public class OrderHistoryResponse
    {
        public bool IsFirstPage { get; set; }
        public int Count { get; set; }
        public List<OrderShortResponse> Orders { get; set; }
        public bool IsLastPage { get; set; }
        public int PageNumber { get; set; }
        public int TotalItemCount { get; set; }
    }
}
