using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Mvc;
using SOAImageGalleryAPI.Models;
using System;
using SOAImageGalleryAPI.Wrappers;
using SOAImageGalleryAPI.Filter;
using SOAImageGalleryAPI.Services;
using SOAImageGalleryAPI.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SOAImageGalleryAPI.Models.Dto.Requests;
using SOAImageGalleryAPI.Models.Dto.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SOAImageGalleryAPI.Models.Dto;

namespace SOAImageGalleryAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        private DataContext _context = null;
        public CommentController(DataContext context)
        {
            _context = context;
        }

        // Add comment to image
        [HttpPost("/image/{id}/comment")]
        public IActionResult AddComment([FromHeader] string Authorization, [FromBody] CommentDto comment, string id)
        {
            if (!TokenDecoder.Validate(Authorization, _context))
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Please login again!",
                    Errors = new[] { "Unauthorized, token not valid!" }
                });
            }

            // check make sure users cant comment others private images
            var img = _context.Images.FirstOrDefault(i => i.Id == id);
            if (img.UserID != TokenDecoder.DecodeUid(Authorization) && img.IsPublic == false)
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Cant access image",
                });
            }

            if (ModelState.IsValid)
            {
                
                try
                {
                    var newComment = new Comment
                    {
                        CommentId = Guid.NewGuid().ToString(),
                        CommentText = comment.CommentText,
                        UserID = TokenDecoder.DecodeUid(Authorization),
                        ImageID = id,
                        Created = DateTime.Now,
                        Updated = DateTime.Now
                    };

                    _context.Comments.Add(newComment);
                    _context.SaveChanges();

                    return Ok(new Response<CommentDto>()
                    {
                        Data = new CommentDto { 
                            CommentId = newComment.CommentId,
                            User = new UserDto
                            {
                                UserId = newComment.UserID
                            },
                            CommentText = newComment.CommentText,
                            ImageId = newComment.ImageID
                        },
                        Succeeded = true,
                        Message = "Comment added"
                    });
                }
                catch (Exception e) 
                {
                    return BadRequest(new Response<CommentDto>()
                    {
                        Succeeded = false,
                        Errors = new[] { e.Message }
                    });
                }
            }
            else
            {
                return BadRequest(new Response<CommentDto>()
                {
                    Data = comment,
                    Succeeded = false,
                    Message = "Check request body",
                    Errors = new[] { "Invalid payload!" }
                });
            }
        }

        // Edit comment
        [HttpPut("/comment/{id}")]
        public IActionResult EditComment([FromHeader] string Authorization, [FromBody] CommentDto
            comment, string id)
        {
            if (!TokenDecoder.Validate(Authorization, _context))
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Please login again!",
                    Errors = new[] { "Unauthorized, token not valid!" }
                });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new Response<CommentDto>()
                {
                    Data = comment,
                    Succeeded = false,
                    Message = "Check request body",
                    Errors = new[] {"Invalid payload!"}
                });
            }

            var existingComment = _context.Comments.FirstOrDefault(i => i.CommentId == id);

            if (existingComment == null)
            {
                return NotFound(new Response<string>()
                {
                    Data = id,
                    Succeeded = false,
                    Errors = new[] { "Couldn't find comment" }
                });
            }

            if (existingComment.UserID != TokenDecoder.DecodeUid(Authorization))
            {
                return BadRequest(new Response<CommentDto>()
                {
                    Succeeded = false,
                    Message = "User can only edit their own comments",
                    Errors = new[] { "Denied access" }
                });
            }

            try
            {
                existingComment.CommentText = comment.CommentText;
                existingComment.Updated = DateTime.Now;
                _context.Comments.Update(existingComment);
                _context.SaveChanges();

                return Ok(new Response<CommentDto>() 
                {
                    Data = new CommentDto { 
                        CommentId = existingComment.CommentId,
                        User = new UserDto { 
                            UserId = existingComment.UserID
                        },
                        CommentText = existingComment.CommentText,
                        ImageId = existingComment.ImageID
                    },
                    Succeeded = true,
                    Message = "Comment edited!"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Response<CommentDto>()
                {
                    Succeeded = false,
                    Errors = new[] { e.Message }
                });
            }
        }
        

        // Delete comment
        [HttpDelete("/comment/{id}")]
        public IActionResult DeleteComment([FromHeader] string Authorization, string id)
        {
            if (!TokenDecoder.Validate(Authorization, _context))
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Please login again!",
                    Errors = new[] { "Unauthorized, token not valid!" }
                });
            }

            var existingComment = _context.Comments.FirstOrDefault(i => i.CommentId == id);

            if (existingComment == null)
            {
                return NotFound(new Response<string>()
                {
                    Data = id,
                    Succeeded = false,
                    Errors = new[] { "Couldn't find comment!" }
                });
            }

            if (existingComment.UserID != TokenDecoder.DecodeUid(Authorization))
            {
                return BadRequest(new Response<string>()
                {
                    Succeeded = false,
                    Message = "User can only delete their own comments",
                    Errors = new[] { "Denied access" }
                });
            }

            try
            {
                _context.Comments.Remove(existingComment);
                _context.SaveChanges();
                return Ok(new Response<CommentDto>() 
                {
                    Succeeded = true,
                    Message = "Comment deleted!"
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Response<CommentDto>()
                {
                    Succeeded = false,
                    Errors = new[] { e.Message }
                });
            }
        }

        // Get all comments (delete later)
        [HttpGet("/comment/all")]
        public ActionResult GetComments()
        {
            return Ok(new Response<List<Comment>>(_context.Comments.ToList()));
        }

    }
}
