using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BarcodeTrackerWEB.Models;
namespace BarcodeTrackerWEB.ViewModels
{
    public class BIDashboardVM

    {
        //This is a View Model that will contain multiple models in order to perform business logic from data sources across multiple models (tables)

        //public Models.Customer customerList { get; set; }

        public List<Customer> customerList = new List<Customer>();

        public List<Models.Order> ordersLists = new List<Models.Order>();

        public List<Models.Order> allOrderList = new List<Models.Order>();

        public int numberOfCustomers { get; set; }
        //public Models.Order OrderVM { get; set; }

        //public Models.OrderDetail OrderDetailVM { get; set; }

        //public Models.SalesRep SalesRepVM { get; set; }

        //public Models.Product ProductVM { get; set; }



    }
}