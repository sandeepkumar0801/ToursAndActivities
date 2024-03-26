using Microsoft.AspNetCore.Identity;

namespace HangFireMVC.Models
{
    public class ApplicationUser : IdentityUser    
    {
        public int PrimalIdentityID { get; set; }
    }
}
