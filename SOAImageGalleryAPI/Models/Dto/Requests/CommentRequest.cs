using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models.Dto.Requests
{
    public class CommentRequest
    {
        [Required]
        public string CommentText { get; set; }
        [Required]
        public string ImageID { get; set; }
        public string CommentId { get; set; }
        
    }
}
