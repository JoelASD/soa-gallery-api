using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models
{
    public class Comment
    {
        public string CommentId { get; set; }
        public string CommentText { get; set; }
        [Required]
        public string UserID { get; set; }
        public User User { get; set; }
        [Required]
        public string ImageID { get; set; }
        public Image Image { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
