using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BarcodeTrackerWEB.Models;

namespace BarcodeTrackerWEB.ViewModels
{
    public class TopProductViewModel
    {
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string Category { get; set; }
        public int CountOrder { get; set; }
        
        public List<OrderDetail> orderDetailsVM { get; set; }
        public string TotalSold { get; set; }
    }
}