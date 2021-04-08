using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models.Dto.Responses
{
    public class SimpleResponse
    {
        public bool Result { get; set; }
        public List<string> Errors { get; set; }
        public string Id { get; set; }
    }
}
