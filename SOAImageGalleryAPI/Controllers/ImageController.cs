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
using SOAImageGalleryAPI.Models.Dto;
using System.IO;
using Microsoft.EntityFrameworkCore;

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
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetAllPagedImages([FromQuery] PaginationFilter filter)
        {
            try
            {
                var route = Request.Path.Value;

                // Creating pagination filter
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                // Getting paged images
                var pagedData = _context.Images
                    .Include(i => i.Votes)
                    .Include(i => i.User)
                    .Where(i => i.IsPublic == true)
                    .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                    .Take(validFilter.PageSize)
                    .ToList();

                var totalRecords = _context.Images.Count(); // Total record count

                // Creating better return type: ImageGto
                var data = new List<ImageDto>();

                foreach (var image in pagedData)
                {
                    var i = new ImageDto
                    {
                        ImageId = image.Id,
                        User = new UserDto
                        {
                            UserId = image.User.Id,
                            UserName = image.User.UserName
                        },
                        ImageFile = image.ImageFile,
                        ImageTitle = image.ImageTitle,
                        VoteSum = image.Votes.Sum(v => v.Voted)
                    };

                    data.Add(i);
                }

                var pagedResponse = PaginationHelper.CreatePagedReponse<ImageDto>(data, validFilter, totalRecords, _uriService, route);

                return Ok(pagedResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Succeeded = false,
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Get all images
        [HttpGet("/image/all")]
        public ActionResult GetImages()
        {
            try
            {
                List<Image> images = _context.Images.ToList(); // Getting all images

                // Converting Images of better return type: ImageDto
                List<ImageDto> data = new List<ImageDto>();

                foreach (var image in images)
                {

                    var i = new ImageDto
                    {
                        ImageId = image.Id,
                        UserId = image.UserID,
                        ImageFile = image.ImageFile,
                        ImageTitle = image.ImageFile,
                        IsPublic = image.IsPublic
                    };

                    data.Add(i);
                }
                return Ok(new Response<List<ImageDto>>()
                {
                    Succeeded = true,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Succeeded = false,
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Adding an image // USER CAN POST PICTURES FOR OTHER USERS -- FIX
        [HttpPost]
        public async Task<ActionResult> AddImage([FromHeader] string Authorization, [FromBody] ImageDto image)
        {
            if (!TokenDecoder.Validate(Authorization, _context)) 
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Please login again!",
                    Errors = new[] { "Unauthorized, token not valid!" }
                });
            }

            if (ModelState.IsValid)
            {
                // Parsing the image content into extension and content
                string[] imageContent = image.ImageFile.Split(',');

                // Converting from base64 to bytes
                Byte[] bytes = Convert.FromBase64String(imageContent[1]);

                MemoryStream stream = new MemoryStream(bytes);

                // Creating the file path
                string filePath = _env.ContentRootPath + @$"\{Guid.NewGuid()}.{imageContent[0]}";

                // Parsing the file name from the file path
                string fileName = filePath.Split("\\")[filePath.Split("\\").Length - 1];

                Image imageToSave = new Image();

                // Adding nesessary properties to the image object 
                imageToSave.Id = Guid.NewGuid().ToString();
                imageToSave.UserID = TokenDecoder.DecodeUid(Authorization);
                imageToSave.Created = DateTime.Now;
                imageToSave.Updated = DateTime.Now;
                imageToSave.ImageFile = fileName;
                imageToSave.ImageTitle = image.ImageTitle;
                imageToSave.IsPublic = image.IsPublic;

                // Uploading an Image to the MinIO bucket
                // Saving image data to the database
                try
                {
                    await _minio.PutObjectAsync("images", fileName, stream, stream.Length);
                    _context.Images.Add(imageToSave);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return BadRequest(new Response<string>()
                    {
                        Message = "Oops! Something is not right...",
                        Succeeded = false,
                        Errors = new[] { ex.Message }
                    });
                }

                return CreatedAtAction(nameof(AddImage), new Response<Image>(imageToSave)
                {
                    Message = "Image has been added succesfully",
                    Succeeded = true,
                    Data = imageToSave
                });
            }
            return BadRequest();
        }

        // Getting one image
        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetOneImage(string id)
        {
            // Getting one image by ID
            var image = _context.Images.FirstOrDefault(i => i.Id == id);

            // CHecking if image is found
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
                // Loading image's comments
                _context.Entry(image).Collection(i => i.Comments).Load();

                _context.Entry(image).Collection(v => v.Votes).Load();

                var data = new ImageDto
                {
                    ImageId = image.Id,
                    UserId = image.UserID,
                    ImageFile = image.ImageFile,
                    ImageTitle = image.ImageFile,
                    VoteSum = image.Votes.Sum(x => x.Voted),
                    Comments = new List<CommentDto>()
                };

                foreach (var c in image.Comments)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == c.UserID);
                    var comment = new CommentDto
                    {
                        CommentId = c.CommentId,
                        User = new UserDto {
                            UserId = user.Id,
                            UserName = user.UserName
                        },
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
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Succeeded = false,
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Editing an image
        [HttpPut]
        public IActionResult EditImage([FromHeader] string Authorization, [FromBody] ImageDto image)
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(new Response<ImageDto>()
                    {
                        Data = image,
                        Message = "Check the request body",
                        Succeeded = false,
                        Errors = new[] { "Request body is incorrect" }
                    });
                }

                var imageToEdit = _context.Images.FirstOrDefault(i => i.Id == image.ImageId);

                if (imageToEdit == null)
                {
                    return NotFound(new Response<string>()
                    {
                        Data = image.ImageId,
                        Succeeded = false,
                        Errors = new[] { "Couldn't find the image" }
                    });
                }

                if (imageToEdit.UserID != TokenDecoder.DecodeUid(Authorization))
                {
                    return BadRequest(new Response<ImageDto>()
                    {
                        Succeeded = false,
                        Message = "User can only edit their own images",
                        Errors = new[] { "Access denied, authorization error" }
                    });
                }


                imageToEdit.ImageTitle = image.ImageTitle;
                imageToEdit.IsPublic = image.IsPublic;
                imageToEdit.Updated = DateTime.Now;
                _context.Images.Update(imageToEdit);
                _context.SaveChanges();

                image.UserId = imageToEdit.UserID;

                return Ok(new Response<ImageDto>()
                {
                    Data = image,
                    Message = "Image updated successfully",
                    Succeeded = true
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Succeeded = false,
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Deleting an image
        [HttpDelete("/image/{id}")]
        public async Task<IActionResult> DeleteImage(string id, [FromHeader] string Authorization)
        {
            if (!TokenDecoder.Validate(Authorization, _context))
            {
                return Unauthorized(new Response<string>()
                {
                    Message = "Please login again!",
                    Errors = new[] { "Unauthorized, token not valid!" }
                });
            }

            string imageFile = "";
            try
            {
                var img = _context.Images.FirstOrDefault(i => i.Id == id);            

                if (img == null)
                {
                    return NotFound(new Response<string>(error: "The image does not exist"));
                }

                if (img.UserID != TokenDecoder.DecodeUid(Authorization))
                {
                    return BadRequest(new Response<string>(error: "Authorization error"));
                }

                _context.Entry(img).Collection(i => i.Comments).Load();
                _context.Entry(img).Collection(i => i.Votes).Load();
                _context.Entry(img).Collection(i => i.Favourites).Load();



                //var commentsToDelete = _context.Comments.Where(c => c.ImageID == id);
                //var favoritesToDelete = _context.Favorites.Where(f => f.ImageID == id);
                //var votesToDelete = _context.Votes.Where(v => v.ImageID == id);

                imageFile = img.ImageFile;
                await _minio.RemoveObjectAsync("images", imageFile);
                //_context.Comments.RemoveRange(commentsToDelete);
                //_context.Favorites.RemoveRange(favoritesToDelete);
                //_context.Votes.RemoveRange(votesToDelete);
                _context.Images.Remove(img);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Succeeded = false,
                    Errors = new[] { ex.Message }
                });
            }
            // fix response
            return Ok(new Response<string>() 
            { 
                Succeeded = true,
                Message = $"Image {imageFile} was deleted with its votes and comments"
            });
        }

        // Getting top 5 trending images within last 24h
        [AllowAnonymous]
        [HttpGet("trending")]
        public IActionResult GetTrendingImages()
        {
            try
            {
                DateTime date = DateTime.UtcNow.Date.AddDays(-1);
                var imagesToReturn = _context.Set<Vote>()
                    .Where(x => x.Updated >= date)
                    .GroupBy(x => x.ImageID)
                    .Select(x => new { ImageID = x.Key, VoteSum = x.Sum(a => a.Voted) })
                    .OrderByDescending(x => x.VoteSum)
                    .Select(x => new ImageDto {
                        ImageId = x.ImageID,
                        VoteSum = x.VoteSum
                    })
                    .Take(5)
                    .ToList();

                List<ImageDto> data = new List<ImageDto>();

                foreach (ImageDto image in imagesToReturn)
                {
                    Image i = _context.Images.FirstOrDefault(i => i.Id == image.ImageId);
                    data.Add(new ImageDto
                    {
                        ImageId = image.ImageId,
                        ImageTitle = i.ImageTitle,
                        ImageFile = i.ImageFile,
                        VoteSum = image.VoteSum
                    });
                }

                return Ok(new Response<List<ImageDto>>()
                {
                    Data = data,
                    Succeeded = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Succeeded = false,
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Getting all the images of the specific user
        [AllowAnonymous]
        [HttpGet("/user/{user_id}/images")]
        public IActionResult GetUserImages([FromHeader] string Authorization, string user_id)
        {
            try
            {
                List<Image> images = new List<Image>();
                
                if (user_id == TokenDecoder.DecodeUid(Authorization) && TokenDecoder.Validate(Authorization, _context))
                {
                    images = _context.Images.Where(i => i.UserID == user_id).ToList();
                }
                else
                {
                    images = _context.Images.Where(i => i.UserID == user_id && i.IsPublic == true).ToList();
                }

                List<ImageDto> data = new List<ImageDto>();

                foreach (var image in images)
                {

                    var i = new ImageDto
                    {
                        ImageId = image.Id,
                        UserId = image.UserID,
                        ImageFile = image.ImageFile,
                        ImageTitle = image.ImageTitle,
                        IsPublic = image.IsPublic
                    };

                    data.Add(i);
                }

                return Ok(new Response<List<ImageDto>>()
                {
                    Succeeded = true,
                    Message = "Success",
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Errors = new[] { ex.Message }
                });
                throw;
            }
        }

        //add image to your favorites
        [HttpPut("{image_id}/favorite")]
        public IActionResult AddImageToFavorites([FromHeader] string Authorization, string image_id)
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
                // Checking if image is already in favorites, if so it will be removed from favorites
                UserHasFavourite favoriteImage = _context.Favorites.FirstOrDefault(f => f.UserID == TokenDecoder.DecodeUid(Authorization) && f.ImageID == image_id);
                if (favoriteImage != null)
                {
                    _context.Favorites.Remove(favoriteImage);
                    _context.SaveChanges();
                    return Ok(new Response<string>()
                    {
                        Message = "Image has been succesfully removed from favorites",
                        Succeeded = true
                    });
                }

                // CHecking if image exists and is available (not someone elses private image)
                Image image = _context.Images.FirstOrDefault(i => i.Id == image_id);
                if (image == null || image.UserID != TokenDecoder.DecodeUid(Authorization) && image.IsPublic == false)
                {
                    return NotFound(new Response<string>()
                    {
                        Message = $"Image with ID: {image_id} does not exist or is private",
                        Succeeded = false
                    });
                }

                // Creating new Favorite Image object and saving it to the database
                UserHasFavourite uHF = new UserHasFavourite()
                {
                    FavouriteID = Guid.NewGuid().ToString(),
                    UserID = TokenDecoder.DecodeUid(Authorization),
                    ImageID = image_id,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                };

                _context.Favorites.Add(uHF);
                _context.SaveChanges();

                return Ok(new Response<string>()
                {
                    Message = "Image has been added to the favorites succesfully",
                    Succeeded = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response<string>()
                {
                    Message = "Oops! Something is not right...",
                    Errors = new[] { ex.Message }
                });
                throw;
            }
        }
    }
}

