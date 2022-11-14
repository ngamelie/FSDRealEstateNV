using System.ComponentModel.DataAnnotations;

namespace RealEstate.Models
{
	public class UserAppModel	{     
        public int id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        
    }
}
