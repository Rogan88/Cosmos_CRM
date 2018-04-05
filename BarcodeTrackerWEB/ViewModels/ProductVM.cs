using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BarcodeTrackerWEB.Models;
namespace BarcodeTrackerWEB.ViewModels
{
    public class ProductVM
    {
        public virtual ICollection<OrderDetail> OrderDetailsVM { get; set; }
        public virtual ICollection<Product> ProductsVM { get; set; }
    }
}