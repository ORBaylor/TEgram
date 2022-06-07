using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone.Models;
using Capstone.DAO;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;

namespace Capstone.Controllers
{
    [Route("posts")]

    [ApiController]
   [Authorize]
    public class PostsController : Controller
    {
        private IPostDao postsDao;
        private readonly IUserDao userDao;
        private IPhotoDao photoDao;
        private ICommentsDao commentsDao;

        private int AuthUser()
        {
            int authUserId = Int32.Parse(User.FindFirst("sub")?.Value);
            return authUserId;
        }

        private string ToBase64Decode(byte[] imgArray)
        {
            //Covert byte array into string so it can be sent back as Json obj


            // byte[] base64EncodedBytes = Convert.FromBase64String(base64EncodedText);

            string img = Convert.ToBase64String(imgArray);
            return img;
        }

        public PostsController(IPostDao postsDao, IUserDao userDao, IPhotoDao photoDao, ICommentsDao commentsDao)
        {
            this.postsDao = postsDao;
            this.userDao = userDao;
            this.photoDao = photoDao;
            this.commentsDao = commentsDao;
        }

        //works
        [HttpPost("upload")]
        public ActionResult<int> Upload([FromForm] IFormFile image)
        {

            try
            {
                IFormFile postedFile = image;


                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

                if (postedFile.Length > 0)
                {
                    string fileName = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');

                    string filetext = ContentDispositionHeaderValue.Parse(postedFile.ContentDisposition).FileName.Trim('"');

                    string finalPath = Path.Combine(uploadFolder, fileName);

                    int photoId = -1;

                    using (FileStream fileStream = new FileStream(finalPath, FileMode.Create))
                    {
                        postedFile.CopyTo(fileStream);
                    }

                    if (image.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            postedFile.CopyTo(ms);
                            byte[] fileByte = ms.ToArray();
                            string data = Convert.ToBase64String(fileByte);
                            photoId = photoDao.SaveFile(data, fileName);


                        }
                    }

                    return Ok(photoId);
                }
                else
                {
                    return BadRequest("The file is not there");
                }
            }
            catch (Exception)
            {

                throw;
            }


        }
        //works
        [HttpPost()]
        public ActionResult<Post> Create(Post post)
        {
            //new
            //post.userId = AuthUser();
            Post newPost = postsDao.CreatePost(post);
            return Created($"/posts/{newPost.postId}", newPost);
        }
        //works
        [HttpGet()]
        public ActionResult<List<TegramPost>> GetAllPosts()
        {
            //    string imgUrl = ToBase64Decode(post.photo);
            //    post.photo = imgUrl;
            List<Post> allPosts = postsDao.GetAllPost();
            List<TegramPost> allTegramPosts = new List<TegramPost>();

            foreach (Post post in allPosts)
            {
                TegramPost tegram = new TegramPost();
                tegram.post = postsDao.GetPostById(post.postId);
                //tegram.username = userDao.GetUsername(AuthUser());
                tegram.username = userDao.GetUsername(post.userId);
                tegram.post.likes = postsDao.GetLikesFromPost(post.postId);
                tegram.post.liked = postsDao.IsPostLikedByUser(post.postId, AuthUser());
                tegram.commentsList = commentsDao.GetAllCommentsFromPost(post.postId);
                tegram.photo = photoDao.GetLikesFromPost(post.postId);
                allTegramPosts.Add(tegram);
            }

            return Ok(allTegramPosts);
        }

        //works
        [HttpGet("user/{userId}")]
        public ActionResult<List<TegramPost>> GetPostsByUser(int userId)
        {
            List<Post> postsByUser = postsDao.GetPostsByUser(userId);
            List<TegramPost> allTegramPostsByUser = new List<TegramPost>();

            foreach (Post post in postsByUser)
            {
                TegramPost tegram = new TegramPost();
                tegram.post = postsDao.GetPostById(post.postId);
                tegram.username = userDao.GetUsername(AuthUser());
                tegram.post.likes = postsDao.GetLikesFromPost(post.postId);
                tegram.post.liked = postsDao.IsPostLikedByUser(post.postId, AuthUser());
                tegram.commentsList = commentsDao.GetAllCommentsFromPost(post.postId);
                tegram.photo = photoDao.GetLikesFromPost(post.postId);
                allTegramPostsByUser.Add(tegram);
            }

            return Ok(allTegramPostsByUser);
        }


        //works
        [HttpGet("{postId}")]
        //****working on this*******
        public ActionResult<TegramPost> GetPostById(int postId)
        {
            TegramPost tegram = new TegramPost();

            tegram.post = postsDao.GetPostById(postId);
            tegram.post.likes = postsDao.GetLikesFromPost(postId);
            tegram.commentsList = commentsDao.GetAllCommentsFromPost(postId);
            tegram.photo = photoDao.GetLikesFromPost(postId);
            if (tegram != null)
            {
                return Ok(tegram);
            }
            else
            {
                return NotFound();

            }
        }

       


        //works
        [HttpGet("photos/{photoId}")]
        public ActionResult<Photo> GetPhotoById(int photoId)
        {
            Photo photo = photoDao.GetPhotoById(photoId);
            if (photo != null)
            {
                return Ok(photo);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPut("{postId}")]
        //does not work
        //Not needed
        public ActionResult UpdateCaption(int postId, string caption)
        {
            Post existingPost = postsDao.GetPostById(postId);
            if (existingPost == null)
            {
                return NotFound();
            }

            postsDao.UpdateCaption(postId, caption);
            return Ok();
        }

        //works
        [HttpGet("user/{userId}/liked")]
        public ActionResult<List<Post>> GetLikedPostsByUser(int userId)
        {
            List<Post> allLikedPostsByUser = postsDao.GetLikedPostsByUser(userId);
            return Ok(allLikedPostsByUser);
        }

        //works
        [HttpGet("user/{userId}/favorited")]
        public ActionResult<List<Post>> GetFavoritedPostsByUser(int userId)
        {
            List<Post> allFavoritedPostsByUser = postsDao.GetFavoritedPostsByUser(userId);
            return Ok(allFavoritedPostsByUser);
        }
        //works
        [AllowAnonymous]
        [HttpPut("favorite/{postId}/{userId}")]
        public ActionResult FavoritePost(int postId, int userId)
        {
            postsDao.AddPhotoToFavoritedList(postId, userId);
            return Ok();
        }
        //works
        [HttpPut("unfavorite/{postId}")]
        public ActionResult UnfavoritePost(int postId)
        {
            postsDao.RemovePhotoFromFavoritedList(postId, AuthUser());
            return Ok();
        }

        [HttpPut("like/{postId}")]
        public ActionResult LikePost(int postId)
        {
            postsDao.LikeAPost(postId, AuthUser());
            return Ok();
        }

        [HttpPut("unlike/{postId}")]
        public ActionResult UnlikePost(int postId)
        {
            postsDao.UnlikeAPost(postId, AuthUser());
            return Ok();
        }
    }
}
