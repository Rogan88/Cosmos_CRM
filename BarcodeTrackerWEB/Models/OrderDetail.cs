namespace BarcodeTrackerWEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderDetail
    {
        [Key]
        public int OrderDetailsId { get; set; }

        //[ForeignKey("OrderId")]
        public int OrderId { get; set; }

        //[ForeignKey("ProductId")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        public int Discount { get; set; }

        public string Description { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }

        public string barcodePrefix { get; set; }

        public string startSequence { get; set; }

        public string endSequence { get; set; }

        public string barcodeSuffix { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
    }
}
