using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Mvc;
using SOAImageGalleryAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using SOAImageGalleryAPI.Wrappers;

namespace SOAImageGalleryAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        /*private DataContext _context = null;
        public UserController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("/register")]
        public ActionResult RegisterUser([FromBody] User user)
        {
            if (_context.Users.FirstOrDefault(i => i.UserName == user.UserName) != null)
            {
                return BadRequest(new Response<User>("User already exists"));
            }

            if (ModelState.IsValid)
            {
                user.UserId = Guid.NewGuid().ToString();
                user.UserPassword = BCrypt.Net.BCrypt.HashPassword(user.UserPassword);
                user.Created = DateTime.Now;
                user.Updated = DateTime.Now;
                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("/login")]
        public IActionResult LogInUser([FromBody] User user)
        {
            var fetchedUser = _context.Users.FirstOrDefault(i => i.UserName == user.UserName);
            if(fetchedUser == null) { return BadRequest(); }

            if (!BCrypt.Net.BCrypt.Verify(user.UserPassword, fetchedUser.UserPassword))
            {
                return BadRequest();
            }
            fetchedUser.UserPassword = null;
            return Ok(new Response<User>(fetchedUser));
        }*/


    }
}
