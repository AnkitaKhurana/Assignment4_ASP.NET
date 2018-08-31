using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Assignment4_ASP.NET.Models;

namespace Assignment4_ASP.NET.Models
{
    [DbConfigurationType(typeof(MyConfiguration))]
    public class Assignment4_ASPNETContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public Assignment4_ASPNETContext() : base("name=Assignment4_ASPNETContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
              modelBuilder.Entity<Customer>().MapToStoredProcedures();
              modelBuilder.Entity<Customer>().Property(p => p.RowVersion).IsConcurrencyToken();
        }

        public System.Data.Entity.DbSet<Assignment4_ASP.NET.Models.Customer> Customers { get; set; }
    }
}