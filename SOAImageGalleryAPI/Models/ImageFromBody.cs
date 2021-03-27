using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models
{
    [Keyless]
    public class ImageFromBody
    {
        public string ImageFile { get; set; }
        public string ImageFileExtension { get; set; }
        public string ImageTitle { get; set; }
        public string UserID { get; set; }
    }
}
