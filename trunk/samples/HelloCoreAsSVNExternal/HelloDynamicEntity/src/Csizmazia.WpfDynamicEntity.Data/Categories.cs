//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Csizmazia.WpfDynamicEntity.Data
{
    public partial class Categories
    {
        public Categories()
        {
            this.Products = new HashSet<Products>();
        }
    
        public int Category_ID { get; set; }
        public string Category_Name { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }
    
        public virtual ICollection<Products> Products { get; set; }
    }
    
}
