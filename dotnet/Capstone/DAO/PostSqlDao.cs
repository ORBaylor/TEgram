using Capstone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Text;
namespace Capstone.DAO
{
    public class PostSqlDao : IPostDao
    {
        private readonly string connectionString;

        public PostSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public Post CreatePost(Post post)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"INSERT INTO posts (user_id, photo_id, caption, likes)
                                                    OUTPUT inserted.post_id
                                                    VALUES (@user_id, @photo_id, @caption, @likes)", conn);

                cmd.Parameters.AddWithValue("@user_id", post.userId);
                cmd.Parameters.AddWithValue("@photo_id", post.photoId);
                cmd.Parameters.AddWithValue("@caption", post.caption);
                cmd.Parameters.AddWithValue("@likes", post.likes);
                //cmd.Parameters.AddWithValue("@upload_time", post.uploadTime);

                post.postId = (int)cmd.ExecuteScalar();

                //SqlCommand cmd2 = new SqlCommand("INSERT INTO liked_posts (post_id, user_id, liked) Values (@post_id, @user_id, 0); "
                //                               + "INSERT INTO favorited_posts (post_id, user_id, favorited) Values (@post_id, @user_id, 0 );", conn);
                //cmd2.Parameters.AddWithValue("@post_id", post.postId);
                //cmd2.Parameters.AddWithValue("@user_id", post.userId);
                //cmd2.ExecuteScalar();
            }

            return GetPostById(post.postId);
        }

        public List<Post> GetAllPost()
        {
            List<Post> posts = new List<Post>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT posts.post_id, posts.user_id, posts.photo_id, posts.caption, posts.likes, posts.upload_time, liked_posts.liked " +
                                                "FROM posts " +
                                                "LEFT JOIN liked_posts ON posts.post_id = liked_posts.post_id " +
                                                "ORDER BY upload_time desc;", conn);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Post post = GetPostFromReader(reader);
                    posts.Add(post);
                }
            }

            return posts;
        }

        public List<Post> GetPostsByUser(int userId)
        {
            List<Post> posts = new List<Post>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT posts.post_id, posts.user_id, posts.photo_id, posts.caption, posts.likes, posts.upload_time, liked_posts.liked " +
                                                "FROM posts " +
                                                "LEFT JOIN liked_posts ON posts.post_id = liked_posts.post_id " +
                                                "WHERE posts.user_id = @user_id " +
                                                "ORDER BY posts.upload_time desc; ", conn);
                //SqlCommand cmd = new SqlCommand("SELECT DISTINCT posts.post_id, posts.user_id, posts.photo_id, posts.caption, posts.likes, posts.upload_time " +
                //                               "FROM posts " +                                        
                //                               "WHERE posts.user_id = @user_id " +
                //                               "ORDER BY posts.upload_time desc; ", conn);

                cmd.Parameters.AddWithValue("@user_id", userId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Post post = GetPostFromReader(reader);
                    posts.Add(post);
                }
            }

            return posts;
        }

        public int GetLikesFromPost(int postId)
        {
            int numberOfLikes;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT COUNT(liked) AS likedCount FROM liked_posts WHERE post_id = @post_id  ", conn);
                cmd.Parameters.AddWithValue("@post_id", postId);

                numberOfLikes = (int)cmd.ExecuteScalar();


            }

            return numberOfLikes;


        }



        public Post GetPostById(int postId)
        {
            Post post = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT posts.post_id, posts.user_id, posts.photo_id, posts.caption, posts.likes, posts.upload_time, liked_posts.liked " +
                                                "FROM posts " +
                                                "Left JOIN liked_posts ON posts.post_id = liked_posts.post_id " +
                                                "WHERE posts.post_id = @post_id;", conn);
                cmd.Parameters.AddWithValue("@post_id", postId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    post = GetPostFromReader(reader);
                }
            }

            return post;
        }

        //needs an Id and a Post object to work
        public void UpdateCaption(int postId, string newCaption)
        {
            Post post = GetPostById(postId);
            post.caption = newCaption;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE post SET caption = @caption WHERE post_id = @post_id", conn);
                    cmd.Parameters.AddWithValue("@caption", post.caption);
                    cmd.Parameters.AddWithValue("@post_id", postId);
                    cmd.ExecuteNonQuery();

                }
            }
            catch (SqlException)
            {
                throw;
            }
            
        }

        public List<Post> GetLikedPostsByUser(int userId)
        {
            List<Post> posts = new List<Post>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT posts.post_id, posts.user_id, posts.photo_id, posts.caption, posts.likes, posts.upload_time, liked_posts.liked " +
                                                "FROM posts " +
                                                "JOIN liked_posts ON posts.post_id = liked_posts.post_id " +
                                                "WHERE (posts.user_id = @user_id AND liked_posts.liked = 1) " +
                                                "ORDER BY upload_time desc;", conn);
                //SqlCommand cmd = new SqlCommand("SELECT posts.post_id, posts.user_id, posts.photo_id, posts.caption, posts.likes, posts.upload_time " +
                //                                "FROM posts " +
                //                                "ORDER BY upload_time desc;", conn);
                cmd.Parameters.AddWithValue("@user_id", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Post post = GetPostFromReader(reader);
                    posts.Add(post);
                }
            }
            return posts;
        }

        public List<Post> GetFavoritedPostsByUser(int userId)
        {
            List<Post> posts = new List<Post>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT posts.post_id, posts.user_id, posts.photo_id, posts.caption, posts.likes, posts.upload_time, favorited_posts.favorited, liked " +
                                                "FROM posts " +
                                                "JOIN favorited_posts ON posts.post_id = favorited_posts.post_id " +
                                                "LEFT JOIN liked_posts ON posts.post_id = liked_posts.post_id " +
                                                " WHERE (posts.user_id = @user_id AND favorited_posts.favorited = 1) " +
                                                "ORDER BY upload_time desc;", conn);
                cmd.Parameters.AddWithValue("@user_id", userId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Post post = GetPostFromReader(reader);
                    posts.Add(post);
                }
            }
            return posts;
        }



        //public void LikeAPost(int postId, int userId)
        //{
        //    using(SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        conn.Open();

        //        SqlCommand cmd = new SqlCommand("insert into liked_posts (post_id, user_id, liked) "
        //                                        + "values (@post_id, @user_id, 1)", conn);

        //        cmd.Parameters.AddWithValue("@post_id", postId);
        //        cmd.Parameters.AddWithValue("@user_id", userId);
        //        cmd.ExecuteNonQuery();
        //    }

        //}

        public void LikeAPost(int postId, int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand check = new SqlCommand("select liked_id from liked_posts where (post_id = @post_id AND user_id = @user_id)", conn);

                check.Parameters.AddWithValue("@post_id", postId);
                check.Parameters.AddWithValue("@user_id", userId);
                SqlDataReader reader = check.ExecuteReader();
                if (!reader.Read())
                {
                    conn.Close();
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("insert into liked_posts (post_id, user_id, liked) "
                                                        + "values (@post_id, @user_id, 1)", conn);

                    cmd.Parameters.AddWithValue("@post_id", postId);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public void UnlikeAPost(int postId, int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("delete liked_posts where post_id = @post_id AND user_id = @user_id", conn);

                cmd.Parameters.AddWithValue("@post_id", postId);
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        //works
        public void AddPhotoToFavoritedList(int postId, int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("insert into favorited_posts (post_id, user_id, favorited) "
                                                + "values (@post_id, @user_id, 1)", conn);

                cmd.Parameters.AddWithValue("@post_id", postId);
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.ExecuteNonQuery();
            }
        }
        //works
        public void RemovePhotoFromFavoritedList(int postId, int userId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("delete favorited_posts where post_id = @post_id AND user_id = @user_id", conn);

                cmd.Parameters.AddWithValue("@post_id", postId);
                cmd.Parameters.AddWithValue("@user_id", userId);
                cmd.ExecuteNonQuery();
            }
        }

        //THIS METHOD TANNER MADE TO CHECK IF CURRNET USER HAS LIKED POST
        public bool IsPostLikedByUser(int postId, int userId)
        {
            List<int> userIdsWhoHaveLikedPost = new List<int>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT user_id FROM liked_posts WHERE post_id = @post_id  ", conn);
                cmd.Parameters.AddWithValue("@post_id", postId);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int likedUserId = Convert.ToInt32(reader["user_id"]);
                    userIdsWhoHaveLikedPost.Add(likedUserId);
                }
                if (userIdsWhoHaveLikedPost.Contains(userId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private Post GetPostFromReader(SqlDataReader reader)
        {
            Post post = new Post()
            {
                postId = Convert.ToInt32(reader["post_id"]),
                userId = Convert.ToInt32(reader["user_id"]),
                photoId = Convert.ToInt32(reader["photo_id"]),
                caption = Convert.ToString(reader["caption"]),
                likes = Convert.ToInt32(reader["likes"]),
                liked = Convert.IsDBNull(reader["liked"]) ? false: (bool) reader["liked"],
                uploadTime = Convert.ToDateTime(reader["upload_time"])
                //uploadTime = DateTime.FromBinary(BitConverter.ToInt64((byte[])reader["upload_time"],0))
                
            };
            return post;
        }

      

    }
}
