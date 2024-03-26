using HangFireMVC.Helper;
using HangFireMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HangFireMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        [Authorize(Roles = "Manager")]
        [Route("signup")]
        public IActionResult Signup()
        {
            return View();
        }
        [Authorize(Roles = "Manager")]
        [Route("signup")]
        [HttpPost]
        public async Task<IActionResult> Signup(SignUpUserModel userModel)
        {
            if (ModelState.IsValid)
            {
                // write your code
                var result = await _accountRepository.CreateUserAsync(userModel);
                if (!result.Succeeded)
                {
                    foreach (var errorMessage in result.Errors)
                    {
                        ModelState.AddModelError("", errorMessage.Description);
                    }

                    return View(userModel);
                }

                ModelState.Clear();
            }

            return View(userModel);
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel signInModel, string returnUrl)
        {
            //if (ModelState.IsValid)
            //{
            var result = await _accountRepository.PasswordSignInAsync(signInModel);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                //return Redirect("/isangodashboard");
                return RedirectToAction("Index", "Home");

            }
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("", "Not allowed to login");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account blocked. Try after some time.");
            }
            else
            {
                ModelState.AddModelError("", "Invalid credentials");
            }

            //}

            return View(signInModel);
        }
        public async Task<IActionResult> SignOut()
        {
            await _accountRepository.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


    }
}
