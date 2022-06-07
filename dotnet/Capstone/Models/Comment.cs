using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Comments
    {
        public int CommentId { get; set; }

        public int UserId { get; set; }

        public string username { get; set; }

        public int PostId { get; set; }

        public string Comment { get; set; }


        public Comments(int commentId, int userid, int postId, string comment)
        {
            this.CommentId = commentId;
            this.UserId = userid;
            this.PostId = postId;
            this.Comment = comment;
        }

        public Comments()
        {

        }
    }
}
