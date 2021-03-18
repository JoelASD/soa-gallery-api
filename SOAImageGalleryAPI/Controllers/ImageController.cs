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
    }
}
