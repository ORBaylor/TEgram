using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Capstone.Models;

namespace Capstone.DAO
{
    public interface ICommentsDao
    {
        List<Comments> GetComments();

        List<Comments> GetAllCommentsFromPost(int postId);

        Comments GetCommentByID(int commentId);

        Comments GetCommentsByUserId(int userId);

        void UpdateCommentById( Comments comments);

        Comments CreateComment(Comments comment);

    }
}
