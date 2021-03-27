using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SOAImageGalleryAPI.Models;
using SOAImageGalleryAPI.Wrappers;
using SOAImageGalleryAPI.Filter;
using SOAImageGalleryAPI.Services;
using SOAImageGalleryAPI.Helpers;
using Minio;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace SOAImageGalleryAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private DataContext _context = null;
        private readonly IUriService _uriService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public ImageController(DataContext context, IUriService uriService, IConfiguration config, IWebHostEnvironment env)
        {
            _context = context;
            _uriService = uriService;
            _config = config;
            _env = env;
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
        public async Task<ActionResult> AddImage([FromBody]ImageFromBody image)
        {
            Image imageToSave = new Image();

            string[] minIoCreds = EnvVars.GetEnvVar(_env.EnvironmentName, _config);

            var minio = new MinioClient(minIoCreds[0], minIoCreds[1], minIoCreds[2]);
            if (ModelState.IsValid)
            {
                Byte[] bytes = Convert.FromBase64String(image.ImageFile);
                string filePath = _env.ContentRootPath + @$"\{Guid.NewGuid()}.{image.ImageFileExtension}";
                try
                {
                    System.IO.File.WriteAllBytes(filePath, bytes);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
                //string file = image.ImageFile.Split("\\")[image.ImageFile.Split("\\").Length - 1];
                //string imageFormat = file.Split('.')[file.Split('.').Length - 1];
                string fileName = filePath.Split("\\")[filePath.Split("\\").Length - 1];
                //string newFileName = String.Format("{0}{1:yyyyMMddHHmmssfff}.{2}", fileName, DateTime.Now, imageFormat);

                // Uploading an Image to the MinIO bucket
                await minio.PutObjectAsync("images", fileName, filePath);

                imageToSave.Id = Guid.NewGuid().ToString();
                imageToSave.Created = DateTime.Now;
                imageToSave.Updated = DateTime.Now;
                imageToSave.ImageFile = fileName;
                imageToSave.UserID = image.UserID;
                imageToSave.ImageTitle = image.ImageTitle;
                _context.Images.Add(imageToSave);
                _context.SaveChanges();
                System.IO.File.Delete(filePath);
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
