using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarcodeTrackerWEB.ViewModels
{
    public class OrderValue
    {
        public int? OrderId { get; set; }
        public decimal Amount { get; set; }

        public int customerId { get; set; }
    }
}