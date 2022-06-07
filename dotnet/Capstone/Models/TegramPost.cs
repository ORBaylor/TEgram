using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class TegramPost
    {
        public string username { get; set; }

        public Post post { get; set; }

        public Photo photo { get; set; }

        public List<Comments> commentsList { get; set; }
    }
}
