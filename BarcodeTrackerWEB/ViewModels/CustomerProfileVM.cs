using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BarcodeTrackerWEB.Models;
using BarcodeTrackerWEB.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace BarcodeTrackerWEB.ViewModels
{
    public class CustomerProfileVM
    {
        [Key]
        public int CustomerProfileVMId { get; set; }

        public Customer CustomerVM { get; set; }
        public CustomerOrder OrderVM { get; set; }

        //attribute for customer rating
        public int rating { get; set; }

        public List<CustomerOrder> recentActivity = new List<CustomerOrder>();
    }

}