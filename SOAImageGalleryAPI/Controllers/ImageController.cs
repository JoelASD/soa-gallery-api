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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using SOAImageGalleryAPI.Models.Dto;

namespace SOAImageGalleryAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private DataContext _context = null;
        private readonly IUriService _uriService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private MinioClient _minio;

        public ImageController(DataContext context, IUriService uriService, IConfiguration config, IWebHostEnvironment env)
        {
            _context = context;
            _uriService = uriService;
            _config = config;
            _env = env;
            _minio = new MinioClient(
                EnvVars.GetEnvVar(_env.EnvironmentName, _config)[0],
                EnvVars.GetEnvVar(_env.EnvironmentName, _config)[1],
                EnvVars.GetEnvVar(_env.EnvironmentName, _config)[2]
                );
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
            Image imageToSave = new Image();

            //string[] minIoCreds = EnvVars.GetEnvVar(_env.EnvironmentName, _config);

            //var minio = new MinioClient(minIoCreds[0], minIoCreds[1], minIoCreds[2]);
            if (ModelState.IsValid)
            {
                // Parsing the image content into extension and content
                string[] imageContent = image.ImageFile.Split(',');

                // Converting from base64 to bytes
                Byte[] bytes = Convert.FromBase64String(imageContent[1]);

                // Creating the file path
                string filePath = _env.ContentRootPath + @$"\{Guid.NewGuid()}.{imageContent[0]}";

                // Saving the file locally
                try
                {
                    System.IO.File.WriteAllBytes(filePath, bytes);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                
                // Parsing the file name from the file path
                string fileName = filePath.Split("\\")[filePath.Split("\\").Length - 1];

                // Adding nesessary properties to the image object 
                image.Id = Guid.NewGuid().ToString();
                image.Created = DateTime.Now;
                image.Updated = DateTime.Now;
                image.ImageFile = fileName;

                // Uploading an Image to the MinIO bucket
                // Saving image data to the database
                try
                {
                    await _minio.PutObjectAsync("images", fileName, filePath);
                    _context.Images.Add(image);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    System.IO.File.Delete(filePath);
                    Console.WriteLine(ex.Message);
                    throw;
                }
                
                // Deleting the image from the local directory
                System.IO.File.Delete(filePath);

                return Ok(new Response<Image>(image)
                {
                    Message = "Image has been added succesfully",
                    Succeeded = true
                });
            }
            return BadRequest();
        }

        // Getting one image
        [HttpGet("{id}")]
        public IActionResult GetOneImage(string id)
        {

            var image = _context.Images.FirstOrDefault(i => i.Id == id);

            if (image == null)
            {
                return NotFound(new Response<string>()
                {
                    Data = id,
                    Succeeded = false,
                    Errors = new[] { "Couldn't find image" }
                });
            }

            try
            {
                _context.Entry(image).Collection(i => i.Comments).Load();

                var data = new ImageDto
                {
                    ImageId = image.Id,
                    UserId = image.UserID,
                    ImageFile = image.ImageFile,
                    ImageTitle = image.ImageFile,
                    Comments = new List<CommentDto>()
                };

                foreach (var c in image.Comments)
                {
                    var comment = new CommentDto
                    {
                        CommentId = c.CommentId,
                        UserId = c.UserID,
                        CommentText = c.CommentText
                    };
                    data.Comments.Add(comment);
                }

                return Ok(new Response<ImageDto>()
                {
                    Data = data,
                    Succeeded = true
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Response<CommentDto>()
                {
                    Succeeded = false,
                    Errors = new[] { e.ToString() }
                });
            }
        }

        // Editing an image
        [HttpPut]
        public async Task<IActionResult> EditImage([FromBody] Image image)
        {
            try
            {
                if (image.ImageFile != null)
                {

                }
                if (ModelState.IsValid)
                {
                    image.Updated = DateTime.Now;
                    _context.Images.Update(image);
                    _context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return BadRequest();
        }

        // Deleting an image
        [HttpDelete("{file_name}")]
        public async Task<IActionResult> DeleteImage(string file_name, [FromHeader] string Authorization)
        {
            string imageFile = "";
            try
            {
                var data = _context.Images.FirstOrDefault(i => i.ImageFile == file_name);

                if (data == null)
                {
                    return NotFound(new Response<string>(error: "The image does not exist"));
                }

                if (data.UserID != TokenDecoder.Decode(Authorization))
                {
                    return BadRequest(new Response<string>(error: "Authorization error"));
                }

                imageFile = data.ImageFile;
                await _minio.RemoveObjectAsync("images", file_name);
                _context.Images.Remove(data);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return Ok(new Response<string>($"Image {imageFile} is deleted succesfully"));
        }
        // Deleting an image
        [HttpGet("/test")]
        public async Task<IActionResult> TestToken([FromHeader] string authorization, string file_name)
        {
            string userId = TokenDecoder.Decode(authorization);
            Console.WriteLine("ID: " + userId);
            return Ok();
        }
    }
}
