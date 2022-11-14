using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RealEstate.Models
{
    public class PropertyAppModel
    {
       public int id { get; set; }       
        public string? Category { get; set; }       
        public string? Description { get; set; }        
        public string? Address { get; set; }       
        public int Price { get; set; }      
        public string? Status { get; set; }       
        public string? Location { get; set; }
        public string? ImageURL { get; set; }
        public IFormFile? Image { get; set; }       
        public string? Owner { get; set; }
    }
}
