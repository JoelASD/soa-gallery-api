using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models
{
    public class User : IdentityUser
    {
        public List<Comment> Comments { get; set; }
        public List<Vote> Votes { get; set; }
        public List<UserHasFavourite> Favourites { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
