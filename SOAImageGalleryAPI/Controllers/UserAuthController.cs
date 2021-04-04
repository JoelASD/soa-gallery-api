﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SOAImageGalleryAPI.Configuration;
using SOAImageGalleryAPI.Models.Dto.Requests;
using SOAImageGalleryAPI.Models.Dto.Responses;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public UserAuthController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
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

                var newUser = new IdentityUser() { Email = user.Email, UserName = user.UserName };
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

        // Generate JWT
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
