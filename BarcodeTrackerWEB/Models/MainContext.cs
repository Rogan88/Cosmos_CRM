namespace BarcodeTrackerWEB.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Collections.Generic;

    public partial class MainContext : DbContext
    {
        public MainContext()
            : base("name=MainContext")
        {

        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SalesRep> SalesReps { get; set; }
        public virtual DbSet<UserAccount> userAccount { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(e => e.Phone);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Province);
                

            modelBuilder.Entity<Customer>()
                .Property(e => e.BillingAddress)
                .IsFixedLength();

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.Customer)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.UnitPrice);
            //.HasPrecision(19, 4);

            modelBuilder.Entity<Order>()
                .Property(e => e.PurchaseOrderNumber)
                .IsFixedLength();

            modelBuilder.Entity<Order>()
                .Property(e => e.TotalAmount)
        .HasPrecision(19, 4);


            modelBuilder.Entity<Order>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.UnitPrice);



            modelBuilder.Entity<OrderDetail>()
            .HasMany(e => e.Products);
            //.WithRequired(e => e.ProductId)
            //.WillCascadeOnDelete(false);



            modelBuilder.Entity<SalesRep>()
        .Property(e => e.FirstName)
        .IsFixedLength();

            modelBuilder.Entity<SalesRep>()
                .Property(e => e.LastName)
                .IsFixedLength();

            modelBuilder.Entity<SalesRep>()
                .Property(e => e.phone);

            modelBuilder.Entity<SalesRep>()
                .HasMany(e => e.Orders)
                .WithRequired(e => e.SalesRep)
                .HasForeignKey(e => e.SaleRepId)
                .WillCascadeOnDelete(false);
        }

        //public System.Data.Entity.DbSet<BarcodeTrackerWEB.ViewModels.OrderDetailVM> OrderDetailVMs { get; set; }
    }
}
