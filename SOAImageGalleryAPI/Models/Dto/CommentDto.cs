using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models.Dto
{
    public class CommentDto
    {
        public string CommentId { get; set; }
        public string UserId { get; set; }
        public string CommentText { get; set; }
        public string ImageId { get; set; }
    }
}
