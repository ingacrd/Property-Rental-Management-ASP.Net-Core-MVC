using Microsoft.Identity.Client;

namespace PropertyRentals.Models
{
    public class VMLogin
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool KeepLogedIn {  get; set; }
    }
}
