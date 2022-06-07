using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Capstone.DAO
{
    public class CommentSqlDao : ICommentsDao
    {
        private readonly string connectionString;

        public CommentSqlDao(string connString)
        {
            connectionString = connString; 
        }

        public Comments CreateComment(Comments comment)
        {
           

            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                //string sqlText = ""
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO comments (user_id, post_id, comment) " +
                                                "OUTPUT INSERTED.comment_id " +
                                                "VALUES (@user_id, @post_id, @comment);", conn);
                cmd.Parameters.AddWithValue("@user_id", comment.UserId);
                cmd.Parameters.AddWithValue("@post_id", comment.PostId);
                cmd.Parameters.AddWithValue("@comment", comment.Comment);

                //newCommentId =
                 comment.CommentId = (int)cmd.ExecuteScalar();
            }


            return GetCommentByID(comment.CommentId);
        }

        public List<Comments> GetAllCommentsFromPost(int postId)
        {

            List<Comments> comments = new List<Comments>();

            using(SqlConnection conn = new SqlConnection(connectionString))
            {


                conn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT comments.post_id, comments.comment, comments.user_id, comments.comment_id, U.username FROM comments JOIN users "+
                            "U ON U.user_id = comments.user_id WHERE comments.post_id = @post_id ", conn);

                cmd.Parameters.AddWithValue("@post_id", postId);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Comments commentsReader = GetCommentsFromReader(reader);
                    comments.Add(commentsReader);
                }

                return comments;

            }


           
        }

        public Comments GetCommentByID(int commentId)
        {
            Comments comments = null;

            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT c.comment_id, c.user_id, c.post_id, c.comment, u.username FROM comments c JOIN users U on "
                                                + "c.user_id = u.user_id WHERE comment_id = @comment_id;", conn);
                //cmd.Parameters.AddWithValue("@city_id", cityId);
                cmd.Parameters.AddWithValue("@comment_id", commentId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    comments = GetCommentsFromReader(reader);
                }
            }
            return comments;
        }

        public List<Comments> GetComments()
        {
            List<Comments> comments = new List<Comments>();

            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT c.comment_id, c.user_id, c.post_id, c.comment, u.username FROM comments c JOIN users U on "
                                                + "c.user_id = u.user_id;", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Comments commentsReader = GetCommentsFromReader(reader);
                    comments.Add(commentsReader);
                }
            }
            return comments;
        }

        public Comments GetCommentsByUserId(int userId)
        {
            Comments comments = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT comment_id, post_id, comment, user_id FROM comments WHERE" +
                    "user_id = @user_id", conn);
                cmd.Parameters.AddWithValue("user_id", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    comments = GetCommentsFromReader(reader);
                }
            }
            return comments;
        }

        public void UpdateCommentById( Comments comments)
        {
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"UPDATE comments SET comment = @comment " +
                    "WHERE comment_id = @comment_id ");
                cmd.Parameters.AddWithValue("@comment_id", comments.CommentId);
                cmd.Parameters.AddWithValue("@comment", comments.Comment);

                cmd.ExecuteNonQuery();
            }

        }



        private Comments GetCommentsFromReader(SqlDataReader reader)
        {
            Comments comment = new Comments();
            comment.CommentId = Convert.ToInt32(reader["comment_id"]);
            comment.UserId = Convert.ToInt32(reader["user_id"]);
            comment.PostId = Convert.ToInt32(reader["post_id"]);
            comment.Comment = Convert.ToString(reader["comment"]);
            comment.username = Convert.ToString(reader["username"]);

            return comment;

        }

    }
}
