using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models
{
    public class UserHasFavourite
    {
        [Key]
        public string FavouriteID { get; set; }
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
