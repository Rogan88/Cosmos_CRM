namespace BarcodeTrackerWEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    

    public partial class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Range(1, 100)]
        //[DataType(DataType.Currency)]
        //[Column(TypeName = "decimal")]


        public decimal UnitPrice { get; set; }



        public int Category { get; set; }

        
    }
}
