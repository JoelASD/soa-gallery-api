﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models.Dto
{
    public class ImageDto
    {
        public string ImageId { get; set; }
        public string UserId { get; set; }
        public string ImageFile { get; set; }
        public string ImageTitle { get; set; }
        public ICollection<CommentDto> Comments { get; set; }
    }
}
