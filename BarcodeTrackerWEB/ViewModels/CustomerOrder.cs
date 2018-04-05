using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BarcodeTrackerWEB.Models;

namespace BarcodeTrackerWEB.ViewModels
{
    public class CustomerOrder
    {
        
        public int CustomerId { get; set; }
        public int? OrderId { get; set; }
        public decimal OrderTotal { get; set; }
        
    }
}