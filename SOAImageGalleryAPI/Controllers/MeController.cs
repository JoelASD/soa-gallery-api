using ConsoleApp.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel;
using SOAImageGalleryAPI.Helpers;
using SOAImageGalleryAPI.Models;
using SOAImageGalleryAPI.Models.Dto;
using SOAImageGalleryAPI.Services;
using SOAImageGalleryAPI.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    public class MeController : Controller
    {
        private DataContext _context = null;
        private readonly IUriService _uriService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private MinioClient _minio;
        public MeController(DataContext context, IUriService uriService, IConfiguration config, IWebHostEnvironment env)
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

        // /me/comments
        [HttpGet("/me/comments")]
        public IActionResult MyComments([FromHeader] string Authorization)
        {
            if (!TokenDecoder.Validate(Authorization, _context))
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Please login again!",
                    Errors = new[] { "Unauthorized, token not valid!" }
                });
            }

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
            if (!TokenDecoder.Validate(Authorization, _context))
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Please login again!",
                    Errors = new[] { "Unauthorized, token not valid!" }
                });
            }

            try
            {
                string uid = TokenDecoder.Decode(Authorization);

                var favoriteImages = _context.Favorites.Where(f => f.UserID == uid).ToList();

                List<ImageDto> data = new List<ImageDto>();

                foreach (var favorite in favoriteImages)
                {
                    Image i = _context.Images.FirstOrDefault(i => i.Id == favorite.ImageID);
                    User u = _context.Users.FirstOrDefault(u => u.Id == i.UserID);
                    data.Add(new ImageDto
                    {
                        ImageId = i.Id,
                        ImageTitle = i.ImageTitle,
                        ImageFile = i.ImageFile,
                        IsPublic = i.IsPublic,
                        User = new UserDto
                        {
                            UserId = u.Id,
                            UserName = u.UserName
                        }
                    });
                }

                return Ok(new Response<List<ImageDto>>()
                {
                    Data = data,
                    Succeeded = true
                });
            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>() 
                {
                    Succeeded = false,
                    Message = "Oops! Somehting is not right...",
                    Errors = new[] { e.Message }
                });
            }

        }

        // /me/favorites/export
        [HttpGet("/me/favorites/export")]
        public async Task<IActionResult> ExportFavorites([FromHeader] string Authorization)
        {
            if (!TokenDecoder.Validate(Authorization, _context))
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Please login again!",
                    Errors = new[] { "Unauthorized, token not valid!" }
                });
            }

            try
            {
                string uid = TokenDecoder.Decode(Authorization);

                var favorites = _context.Favorites.Where(f => f.UserID == uid).ToList();

                List<ZipItem> imgList = new List<ZipItem>();

                foreach (var image in favorites)
                {
                    Image img = _context.Images.FirstOrDefault(i => i.Id == image.ImageID);

                    // Checks if image exists and throws exception in case not.
                    await _minio.StatObjectAsync("images", img.ImageFile);

                    MemoryStream ms = new();

                    await _minio.GetObjectAsync("images", img.ImageFile,
                        (stream) =>
                        {
                            stream.CopyTo(ms);
                        });

                    ms.Position = 0;

                    imgList.Add(new ZipItem(filename: img.ImageFile, content: ms));
                }

                var zipStream = Zipper.Zip(imgList);

                return File(zipStream, "application/octet-stream", fileDownloadName: "favorites.zip");

            }
            catch (Exception e)
            {
                return BadRequest(new Response<string>() 
                {
                    Succeeded = false,
                    Message = "Oops! Somehting is not right...",
                    Errors = new[] { e.Message }
                });
            }
        }

    }
}
