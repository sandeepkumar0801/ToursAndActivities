using HangFireMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace HangFireMVC.Helper
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            //RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //_roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel)
        {
            var user = new ApplicationUser()
            {

                PrimalIdentityID = userModel.PrimalIdentityId,
                Email = userModel.Email,
                UserName = userModel.UserName

            };
            var result = await _userManager.CreateAsync(user, userModel.Password);
            return result;
        }
        public async Task<SignInResult> PasswordSignInAsync(LoginViewModel signInModel)
        {
            return await _signInManager.PasswordSignInAsync(signInModel.UserName, signInModel.Password, signInModel.RememberMe, true);
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

    }
}
