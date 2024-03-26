using Bumblebee.Models;
using Google.Apis.Auth.OAuth2.Responses;
using Isango.Entities;
using Isango.Persistence.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Identity;

namespace Bumblebee.Controllers
{
    [Route("")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IAffiliatePersistence _affiliatePersistence;


        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IAffiliatePersistence affiliatePersistence)
        {
            _userManager = userManager;
            _configuration = configuration;
            _affiliatePersistence = affiliatePersistence;
        }
        [Route("/Token")]
        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Token" })]
        public async Task<IActionResult> Token([FromForm] Login login)
        {
            try
            {
                
                if (login.username != null)
                {
                    try
                    {
                        var tokenResponse = await GetToken(login.username, login.Password, login.grant_type);

                        if (tokenResponse != null)
                        {
                            return Ok(tokenResponse);
                        }

                        return BadRequest(new Token
                        {
                            error = "invalid_grant",
                            error_description = "Invalid credentials"
                        });
                    }
                    catch(Exception ex)
                    {
                        return BadRequest(new Token
                        {
                            error = "invalid_grant",
                            error_description = "Invalid credentials"
                        });
                    }
                }
                else
                {
                    try
                    {
                        using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
                        {
                            string requestBody = await reader.ReadToEndAsync();

                            var keyValuePairs = ParseQueryString(requestBody);

                            var username = keyValuePairs.GetValueOrDefault("username");
                            var password = keyValuePairs.GetValueOrDefault("password");
                            var grant_type = keyValuePairs.GetValueOrDefault("grant_type");


                            var tokenResponse = await GetToken(username, password, grant_type);

                            if (tokenResponse != null)
                            {
                                return Ok(tokenResponse);
                            }

                            return BadRequest(new TokenAuthorization
                            {
                                error = "invalid_grant",
                                error_description = "Invalid credentials"
                            });
                        }
                    }
                    catch(Exception ex)
                    {
                        return BadRequest(new TokenAuthorization
                        {
                            error = "invalid_grant",
                            error_description = "Invalid credentials"
                        });

                    }

                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return BadRequest(new Token
                {
                    error = "server_error",
                    error_description = ex.Message
                });
            }
        }


       private async Task<Token> GetToken(string username, string password,string grant_type)
       {
            if(grant_type== null || grant_type != "password")
            {
                return null;

            }
            var user = await _userManager.FindByNameAsync(username);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var roleName = (await _userManager.GetRolesAsync(user))[0];
                var affiliateId = _affiliatePersistence.GetAffiliateIdByUserId(user.Id);
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim("sub", user.UserName),
                    new Claim("role", roleName),
                    new Claim("availtoken", Guid.NewGuid().ToString()),
                    new Claim("affiliateId", affiliateId)
                };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var bearer = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:TokenExpiryInMinutes"])),
                    signingCredentials: credentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(bearer);
                var tokenResponse = new Token
                {
                    access_token = tokenString,
                    token_type = "bearer",
                    expires_in = Convert.ToInt32(_configuration["Jwt:TokenExpiryInMinutes"]) * 60, // in seconds
                    userName = user.UserName,
                    userId = user.Id,
                    Issued = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"), // Current UTC time
                    Expires = bearer.ValidTo.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'") // Use the expiration time from the token

                };
                return tokenResponse;
            }
            return null;
                
       }


        private Dictionary<string, string> ParseQueryString(string queryString)
        {
            var keyValuePairs = queryString
                .Split('&')
                .Select(part => part.Split('='))
                .ToDictionary(split => Uri.UnescapeDataString(split[0]), split => Uri.UnescapeDataString(split[1]));

            return keyValuePairs;
        }

        
        [Route("/Authorization")]

        [HttpPost]
        [SwaggerOperation(Tags = new[] { "Activity Lite" })]
        public async Task<IActionResult> Authorization(LoginAuthorization login)
        {
            var user = await _userManager.FindByNameAsync(login.username);

            if (user != null)
            {
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, login.Password);

                if (isPasswordValid)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    var tokenResponse = new TokenResponseAuthorization
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        Roles = roles.ToList()
                    };

                    return Ok(tokenResponse);
                }
                else
                {
                    return StatusCode(401, new TokenAuthorization
                    {
                        error = "invalid_grant",
                        error_description = "Invalid password"
                    });
                }
            }
            else
            {
                return StatusCode(400, new TokenAuthorization
                {
                    error = "invalid_request",
                    error_description = "Invalid username"
                });
            }
        }





    }
}
