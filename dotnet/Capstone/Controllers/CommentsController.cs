using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone.Models;
using Capstone.DAO;
using Microsoft.AspNetCore.Authorization;

namespace Capstone.Controllers
{
    [Route("comments")]
    
    [ApiController]
    //[Authorize]
    public class CommentsController : Controller
    {
        private ICommentsDao commentsDao;
        private readonly IUserDao userDao;

        private int AuthUser()
        {
            int authUserId = Int32.Parse(User.FindFirst("sub")?.Value);

            return authUserId;

            
        }

        
        

        public CommentsController(ICommentsDao commentsDao, IUserDao userDao)
        {
            this.commentsDao = commentsDao;
            this.userDao = userDao;
        }

        [HttpPost()]
        public ActionResult<Comments> CreatComment(Comments comments)
        {
            //comments.UserId = AuthUser();
            Comments newComments = commentsDao.CreateComment(comments);
            return Created($"/comments/{newComments.CommentId}", newComments);

        }


        //Works
        [HttpGet("{userId}")]
        public ActionResult<Comments> GetCommentById()
        {
           // string authUser = User.Identity.Name;
            
            

            Comments comments = commentsDao.GetCommentsByUserId(AuthUser());
            return comments;
        }

        //works
        [HttpGet("post/{postId}")]
        public List<Comments> ListOfCommentsByPostId(int postId)
        {
            List<Comments> comments = commentsDao.GetAllCommentsFromPost(postId);

            return comments;

        }




    }
}
