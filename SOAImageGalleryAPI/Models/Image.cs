using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models
{
    public class Image
    {
        public string Id { get; set; }
        public string ImageFile { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
