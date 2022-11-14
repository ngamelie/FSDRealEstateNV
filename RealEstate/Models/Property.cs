using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
    public partial class Property
    {
        public int id { get; set; }

        public int category_id { get; set; }

        public int owner_id { get; set; }

        public string Address { get; set; }

        public int price { get; set; }
        public int status { get; set; }

        public string description { get; set; }

        public string location { get; set; }

        //public virtual Category? Category { get; set; }
        //public virtual User? Owner { get; set; }
    }
}
