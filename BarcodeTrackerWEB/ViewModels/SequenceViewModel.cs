using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BarcodeTrackerWEB.ViewModels
{
    public class SequenceViewModel
    {
        public List<CustomerVM> Customer { get; set; }
        public List<OrderDetailVM> OrderDetails { get; set; }
        //public int ClientId { get; set; }
        //public string ClientName { get; set; }
        //public string BarcodePrefix { get; set; }
        //public string BarcodeSequence { get; set; }
        //public string BarcodeSuffix { get; set; }

    }
}