using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SOAImageGalleryAPI.Models;
using SOAImageGalleryAPI.Wrappers;
using SOAImageGalleryAPI.Filter;
using SOAImageGalleryAPI.Services;
using SOAImageGalleryAPI.Helpers;
using Minio;
using Microsoft.Extensions.Configuration;

namespace SOAImageGalleryAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private DataContext _context = null;
        private readonly IUriService _uriService;
        private readonly IConfiguration _config;
        public ImageController(DataContext context, IUriService uriService, IConfiguration config)
        {
            _context = context;
            _uriService = uriService;
            _config = config;
        }

        // Getting paged images, max 10
        [HttpGet]
        public ActionResult GetAllPagedImages([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var pagedData = _context.Images
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToList();
            var totalRecords = _context.Images.Count();
            var pagedResponse = PaginationHelper.CreatePagedReponse<Image>(pagedData, validFilter, totalRecords, _uriService, route);
            return Ok(pagedResponse);
        }

        // Get all images
        [HttpGet("/image/all")]
        public ActionResult GetImages()
        {
            return Ok(new Response<List<Image>>(_context.Images.ToList()));
        }

        // Adding an image
        [HttpPost]
        public async Task<ActionResult> AddImage([FromBody]Image image)
        {
            var minio = new MinioClient(_config["MinIOHost"], _config["MinIOAccessKey"], _config["MinIOHostSecretKey"]);
            if (ModelState.IsValid)
            {
                string file = image.ImageFile.Split("\\")[image.ImageFile.Split("\\").Length - 1];
                string imageFormat = file.Split('.')[file.Split('.').Length - 1];
                string fileName = file.Split('.')[0];
                string newFileName = String.Format("{0}{1:yyyyMMddHHmmssfff}.{2}", fileName, DateTime.Now, imageFormat);

                // Uploading an Image to the MinIO bucket
                await minio.PutObjectAsync("images", newFileName, image.ImageFile);

                image.Id = Guid.NewGuid().ToString();
                image.Created = DateTime.Now;
                image.Updated = DateTime.Now;
                image.ImageFile = newFileName;
                _context.Images.Add(image);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        // Getting one image
        [HttpGet("{id}")]
        public IActionResult GetOneImage(string id)
        {
            var image = _context.Images.FirstOrDefault(i => i.Id == id);
            return Ok(new Response<Image>(image));
        }

        // Editing an image
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

        // Deleting an image
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
