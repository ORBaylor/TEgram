using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAO
{
    public interface IPostDao
    {
        List<Post> GetAllPost();

        Post GetPostById(int Postid);

        List<Post> GetPostsByUser(int userId);

        void UpdateCaption(int Postid, string newCaption);

        Post CreatePost(Post post);

        List<Post> GetLikedPostsByUser(int userId);

        List<Post> GetFavoritedPostsByUser(int userId);

        void LikeAPost(int postId, int userId);

        void UnlikeAPost(int postId, int userId);

        void AddPhotoToFavoritedList(int postId, int userId);

        void RemovePhotoFromFavoritedList(int postId, int userId);

        int GetLikesFromPost(int postId);

        bool IsPostLikedByUser(int postId, int userId);

    }
}
