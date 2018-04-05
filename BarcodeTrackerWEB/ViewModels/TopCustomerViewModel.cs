using BarcodeTrackerWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarcodeTrackerWEB.ViewModels
{
    public class TopCustomerViewModel
    {


        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerProvince { get; set; }
        public int CountOrder { get; set; }
        public byte[] Logo { get; set; }
        public List<Customer> customerVM { get; set; }
        public string TotalValue { get; set; }
    }

}