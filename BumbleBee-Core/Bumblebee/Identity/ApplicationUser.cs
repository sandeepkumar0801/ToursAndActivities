using Microsoft.AspNetCore.Identity;

namespace WebAPI.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public int PrimalIdentityID { get; set; }
    }
}
