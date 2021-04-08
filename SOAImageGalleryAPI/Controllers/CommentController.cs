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

namespace SOAImageGalleryAPI.Controllers
{
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
        //[HttpPost("/image/{id}/comment")]
        [HttpPost]
        public IActionResult AddComment([FromBody] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CommentId = Guid.NewGuid().ToString();
                comment.Created = DateTime.Now;
                comment.Updated = DateTime.Now;
                _context.Comments.Add(comment);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        // Edit comment
        //[HttpPut("/comment/{id}")]
        [HttpPut]
        public IActionResult EditComment([FromBody] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Updated = DateTime.Now;
                _context.Comments.Update(comment);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
        
        // Get all images (not really needed)
        [HttpGet("/comment/all")]
        public ActionResult GetComments()
        {
            return Ok(new Response<List<Comment>>(_context.Comments.ToList()));
        }

        // Delete comment
        [HttpDelete("/comment/{id}")]
        public IActionResult DeleteComment(string id)
        {
            var comment = _context.Comments.FirstOrDefault(i => i.CommentId == id);
            if(comment == null)
            {
                return NotFound();
            }
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return Ok();
        }
    }
}
