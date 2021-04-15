using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SOAImageGalleryAPI.Helpers;
using SOAImageGalleryAPI.Models;
using SOAImageGalleryAPI.Models.Dto;
using SOAImageGalleryAPI.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class MeController : Controller
    {
        private DataContext _context = null;
        public MeController(DataContext context)
        {
            _context = context;
        }

        // /me/comments
        [HttpGet("/me/comments")]
        public IActionResult MyComments([FromHeader] string Authorization)
        {
            try
            {
                string uid = TokenDecoder.Decode(Authorization);

                var myComments = _context.Comments.Where(c => c.UserID == uid).ToList();
                var myUser = _context.Users.FirstOrDefault(u => u.Id == uid);

                var user = new UserDto
                {
                    UserId = uid,
                    UserName = myUser.UserName
                };

                var data = new List<CommentDto>();

                foreach (var c in myComments)
                {
                    var newCom = new CommentDto
                    {
                        CommentId = c.CommentId,
                        CommentText = c.CommentText,
                        ImageId = c.ImageID,
                        User = user
                    };

                    data.Add(newCom);
                }

                return Ok(new Response<List<CommentDto>>() 
                { 
                    Data = data,
                    Succeeded = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>() 
                { 
                    Succeeded = false,
                    Errors = new[] { e.Message }
                });
            }
        }

        // /me/favorites
        [HttpGet("/me/favorites")]
        public IActionResult MyFavorites([FromHeader] string Authorization)
        {
            try
            {
                string uid = TokenDecoder.Decode(Authorization);
                //var favorites
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest();
            }

        }

        // /me/favorites/export

    }
}
