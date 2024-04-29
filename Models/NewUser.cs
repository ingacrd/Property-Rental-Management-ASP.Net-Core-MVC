using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyRentals.Models
{
    public class NewUser
    {
        public int UserId { get; set; }
        public int TenantId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Photo { get; set; }

        [NotMapped] // This property is not mapped to the database
        public IFormFile PhotoFile { get; set; }

    }
}
