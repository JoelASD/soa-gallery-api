using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SOAImageGalleryAPI.Configuration;
using SOAImageGalleryAPI.Helpers;
using SOAImageGalleryAPI.Models;
using SOAImageGalleryAPI.Models.Dto.Requests;
using SOAImageGalleryAPI.Models.Dto.Responses;
using SOAImageGalleryAPI.Wrappers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtConfig _jwtConfig;
        private readonly DataContext _context;

        public UserAuthController(UserManager<User> userManager, IOptionsMonitor<JwtConfig> optionsMonitor, DataContext context)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            _context = context;
        }

        // Register user
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationRequest user)
        {
            if(ModelState.IsValid)
            {
                //var userExists = await _userManager.FindByEmailAsync(user.Email);
                if(await _userManager.FindByEmailAsync(user.Email) != null)
                {
                    return BadRequest(new RegistrationResponse(){
                        Errors = new List<string>()
                        {
                            "Email already exists!"
                        },
                            Result = false
                    });
                }

                var newUser = new User() { Email = user.Email, UserName = user.UserName };
                var isCreated = await _userManager.CreateAsync(newUser, user.Password);

                if(isCreated.Succeeded)
                {
                    var jwtToken = GenerateJwt(newUser);
                    return Ok(new RegistrationResponse()
                    {
                        Result = true,
                        Token = jwtToken,
                        Id = newUser.Id
                    });

                } else
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                        Result = false
                    }) ;
                }
            }

            return BadRequest(new RegistrationResponse(){
                Errors = new List<string>()
                {
                    "Errors"
                },
                Result = false
            });
        }

        [HttpPost("/login")]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginRequest user) 
        {
            if(ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    return BadRequest(new RegistrationResponse() // create own response, loginResponse?
                    {
                        Errors = new List<string>()
                        {
                            "User doesn't exist"
                        },
                        Result = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if (!isCorrect)
                {
                    return BadRequest(new RegistrationResponse() // create own response, loginResponse?
                    {
                        Errors = new List<string>()
                        {
                            "Invalid password"
                        },
                        Result = false
                    });
                }

                var jwtToken = GenerateJwt(existingUser);

                return Ok(new RegistrationResponse()
                {
                    Result = true,
                    Token = jwtToken
                });
            }

            return BadRequest(new RegistrationResponse() // create own response, loginResponse?
            {
                Errors = new List<string>()
                        {
                            "Invalid payload"
                        },
                Result = false
            });
        }

        [HttpPost("/logout")] // korjaa paremmaksi
        public IActionResult Logout([FromHeader] string Authorization)
        {
            if (TokenDecoder.Validate(Authorization, _context))
            {
                AuthenticationHeaderValue.TryParse(Authorization, out var headerValue);

                var t = new JwtSecurityToken(headerValue.Parameter);
                string expDate = t.Claims.First(c => c.Type == "exp").Value;

                var ed = Int32.Parse(expDate);

                //DateTime date = DateTime.ParseExact(expDate);

                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

                dateTime = dateTime.AddSeconds(ed);

                JwtBlacklist token = new JwtBlacklist()
                {
                    Token = headerValue.Parameter,
                    Created = DateTime.Now,
                    Expires = dateTime
                };

                // fix needed
                _context.Blacklist.Add(token);
                _context.SaveChanges();

                return Ok(new Response<DateTime>() { Data = dateTime, Succeeded = true });
            }

            return BadRequest(new Response<DateTime>() { Succeeded = false, Message = "vittu" });
        }

        [Route("/google")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Signin(string returnUrl)
        
        {
            return new ChallengeResult(
                GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
                });
        }

        
        [Route("/signin-callback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> GoogleCallback(string returnUrl)
        {
            try
            {
                var authenticateResult = await HttpContext.AuthenticateAsync("Google");

                if (!authenticateResult.Succeeded)
                {
                    return Unauthorized(new Response<string>()
                    {
                        Message = "Google Authentication failed!",
                        Succeeded = false
                    });
                }
                else
                {
                    List<string> googleData = new List<string>();

                    foreach (var i in authenticateResult.Principal.Identities)
                    {
                        foreach (var x in i.Claims)
                        {
                            googleData.Add(x.Value.ToString());
                        }
                    }

                    var existingUser = await _userManager.FindByEmailAsync(googleData[0]);
                    if (existingUser != null)
                    {
                        var jwtToken = GenerateJwt(existingUser);

                        return Ok(new RegistrationResponse()
                        {
                            Result = true,
                            Token = jwtToken
                        });
                    }
                    else
                    {
                        var newUser = new User() { Email = googleData[0], UserName = googleData[0].Split("@")[0] };
                        var isCreated = await _userManager.CreateAsync(newUser, $"P{Guid.NewGuid()}");

                        if (isCreated.Succeeded)
                        {
                            var jwtToken = GenerateJwt(newUser);
                            return Ok(new RegistrationResponse()
                            {
                                Result = true,
                                Token = jwtToken,
                                Id = newUser.Id
                            });
                        }
                        else
                        {
                            return BadRequest(new RegistrationResponse()
                            {
                                Errors = isCreated.Errors.Select(x => x.Description).ToList(),
                                Result = false
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Succeeded = false,
                    Errors = new[] { ex.Message }
                });
            }
        }

        private string GenerateJwt(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.Now.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
