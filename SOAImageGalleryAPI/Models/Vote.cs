using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models
{
    public class Vote
    {
        public string VoteId { get; set; }
        public int Voted { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
        public string ImageID { get; set; }
        public Image Image { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
