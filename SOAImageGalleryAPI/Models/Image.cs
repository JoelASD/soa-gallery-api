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
        public string ImageTitle { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public List<Vote> Votes { get; set; }
        public List<UserHasFavourite> Favourites { get; set; }
        public List<Comment> Comments { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        
    }
}
