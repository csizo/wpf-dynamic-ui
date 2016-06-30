﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Data
{
    public partial class DbEntities : DbContext
    {
    	#region SavingChanges
    	public event EventHandler SavingChanges;
        public override int SaveChanges()
        {
            if (SavingChanges != null)
                SavingChanges(this, EventArgs.Empty);
            return base.SaveChanges();
        }
    	#endregion
    	
        public DbEntities()
            : base("name=DbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
    
    }
}
