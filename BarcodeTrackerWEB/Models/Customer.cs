namespace BarcodeTrackerWEB.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;


    public enum ProvinceName
    {
        Gauteng,
        Limpopo,
        Mpumalanga,
        North_West,
        Western_Cape,
        Eastern_Cape,
        Northern_Cape,
        KwazuluNatal,
        Free_State,

    }

    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            Orders = new HashSet<Order>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        public byte[] Logo { get; set; }

        public string Phone { get; set; }

        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        //[Required]
        //[StringLength(20)]
        public ProvinceName Province { get; set; }

        [StringLength(100)]
        public string BillingAddress { get; set; }

        public int Rating { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
