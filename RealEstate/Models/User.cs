using System;
using System.Collections.Generic;

namespace RealEstate.Models
{
    public class User {        

        public int id { get; set; }
        public string? email { get; set; }
        public string? password { get; set; }
        public int? role { get; set; }
        public string? name { get; set; }
        public string? phone { get; set; }        
    }
}
