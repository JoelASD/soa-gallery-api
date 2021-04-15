using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models.Dto
{
    public class VoteDto
    {
        public string VoteId { get; set; }
        [Required]
        [RegularExpression(@"[-]?(1)",
            ErrorMessage = "Invalid voted value")]
        public int Voted { get; set; }
        public string UserId { get; set; }
        public string ImageId { get; set; }
    }
}
