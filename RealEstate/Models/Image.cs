using System;
using System.Collections.Generic;

namespace RealEstate.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public int? property_id { get; set; }
        public string? imageUrl { get; set; }
    }
}
