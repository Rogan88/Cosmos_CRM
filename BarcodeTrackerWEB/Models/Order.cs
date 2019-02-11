namespace BarcodeTrackerWEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public int SaleRepId { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime OrderDate { get; set; }
        
        [StringLength(10)]
        public string PurchaseOrderNumber { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime PurchaseOrderDate { get; set; }

        [Column(TypeName = "money")]
        public decimal TotalAmount { get; set; }

        public int Status { get; set; }

        public string StatusName { get; set; }

        public byte[] PurchaseOrderDoc { get; set; }

        public byte[] SignedLabelProofDoc { get; set; }

        public byte[] Invoice { get; set; }
        
        public virtual Customer Customer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual SalesRep SalesRep { get; set; }
    }
}
