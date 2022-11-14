using System;
using System.Collections.Generic;

namespace RealEstate.Models
{
    public partial class Category
    {
        public int Id { get; set; }
        public string? categoryName { get; set; }

        //public virtual ICollection<Property> Properties { get; set; }
    }
}
