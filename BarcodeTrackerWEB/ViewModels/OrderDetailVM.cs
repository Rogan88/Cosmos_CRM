using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BarcodeTrackerWEB.Models;
using System.ComponentModel.DataAnnotations;

namespace BarcodeTrackerWEB.ViewModels
{
    public class OrderDetailVM
    {
        public int OrderId { get; set; }

        [Key]
        public int OrderDetailsId { get; set; }

        public List<OrderDetail> orderItems { get; set; }
        
        public int ProductId { get; set; }

        public string Description { get; set; }
        
        public Product Item { get; set; }

        public int Quantity { get; set; }
        
        public decimal UnitPrice { get; set; }

        public int Discount { get; set; }


        public string barcodePrefix { get; set; }

        public string startSequence { get; set; }

        public string endSequence { get; set; }

        public string barcodeSuffix { get; set; }


        public List<Product> ProductList { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

        public virtual Customer Customer { get; set; }
    }
}