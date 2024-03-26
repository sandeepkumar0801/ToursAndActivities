using HangFireMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace HangFireMVC.Helper
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(SignUpUserModel userModel);
        Task<SignInResult> PasswordSignInAsync(LoginViewModel signInModel);
        Task SignOutAsync();


    }
}
