using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SOAImageGalleryAPI.Models;

namespace SOAImageGalleryAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private DataContext _context = null;
        public ImageController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetImages()
        {
            return Ok(_context.Images.ToList());
        }

        [HttpPost]
        public ActionResult AddImage([FromBody]Image image)
        {
            if (ModelState.IsValid)
            {
                image.Id = Guid.NewGuid().ToString();
                image.Created = DateTime.Now;
                image.Updated = DateTime.Now;
                _context.Images.Add(image);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        public Image GetOneImage(string id)
        {
            return _context.Images.FirstOrDefault(i => i.Id == id);
        }

        [HttpPut]
        public IActionResult EditImage([FromBody] Image image)
        {
            if (ModelState.IsValid)
            {
                image.Updated = DateTime.Now;
                _context.Images.Update(image);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteImage(string id)
        {
            var data = _context.Images.FirstOrDefault(i => i.Id == id);
            if(data == null)
            {
                return NotFound();
            }
            _context.Images.Remove(data);
            _context.SaveChanges();
            return Ok();
        }
    }
}
