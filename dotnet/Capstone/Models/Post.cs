using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Post
    {
        public int postId { get; set; }

        public int userId { get; set; }

        public int photoId { get; set; }

        public string caption { get; set; }
        public bool liked { get; set; } = false;

        public int likes { get; set; } = 0;

        public DateTime uploadTime { get; set; } //= DateTime.Now;
    }
}
