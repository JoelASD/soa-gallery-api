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
    public class VoteController : Controller
    {
        private DataContext _context = null;
        public VoteController(DataContext context)
        {
            _context = context;
        }

        [HttpPut("/image/{id}/vote")]
        public IActionResult Vote([FromHeader] string Authorization, [FromBody] VoteDto
            vote, string id)
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
                    Succeeded = false,
                    Errors = new[] { "Invalid payload!" }
                });
            }

            string uid = TokenDecoder.DecodeUid(Authorization);

            var existingVote = _context.Votes.FirstOrDefault(v => v.UserID == uid && v.ImageID == id);

            if (existingVote == null)
            {
                try
                {
                    var newVote = new Vote
                    {
                        VoteId = Guid.NewGuid().ToString(),
                        Voted = vote.Voted,
                        UserID = uid,
                        ImageID = id,
                        Created = DateTime.Now,
                        Updated = DateTime.Now
                    };

                    _context.Votes.Add(newVote);
                    _context.SaveChanges();

                    return Ok(new Response<VoteDto>()
                    {
                        Data = new VoteDto
                        {
                            VoteId = newVote.VoteId,
                            Voted = newVote.Voted,
                            UserId = uid,
                            ImageId = id
                        },
                        Succeeded = true,
                        Message = "Vote added!"
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
                //check previous vote
                if (existingVote.Voted == vote.Voted)
                {
                    try
                    {
                        _context.Votes.Remove(existingVote);
                        _context.SaveChanges();
                        
                        return Ok(new Response<VoteDto>()
                        {
                            Succeeded = true,
                            Message = "Vote deleted"
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
                    try
                    {
                        existingVote.Voted = vote.Voted;
                        existingVote.Updated = DateTime.Now;
                        _context.Votes.Update(existingVote);
                        _context.SaveChanges();

                        return Ok(new Response<VoteDto>()
                        {
                            Data = new VoteDto
                            {
                                VoteId = existingVote.VoteId,
                                Voted = existingVote.Voted,
                                UserId = existingVote.UserID,
                                ImageId = existingVote.ImageID
                            },
                            Succeeded = true,
                            Message = "Vote changed"
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
            }
        }

    }
}
