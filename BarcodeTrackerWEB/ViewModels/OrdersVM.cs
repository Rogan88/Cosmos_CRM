using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BarcodeTrackerWEB.Models;
namespace BarcodeTrackerWEB.ViewModels
{
    public class OrdersVM
    {

        public Customer CustomerVM { get; set; }
        public CustomerOrder OrderVM { get; set; }

        public OrderDetail OrderDetailVM { get; set; }

        public SalesRep SalesRepVM { get; set; }

        public Product ProductVM { get; set; }


    }
}