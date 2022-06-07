using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Capstone.Models;
using Capstone.DAO;
using Microsoft.AspNetCore.Http;
namespace Capstone.DAO
{

    public class PhotoSqlDao : IPhotoDao
    {
        private readonly string connectionString;

        public PhotoSqlDao(string connString)
        {
            connectionString = connString;
        }
        public Photo CreatePhoto(string file)
        {



            int newPhotoId;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO photos (photo) " +
                    "OUTPUT INSERTED.photo_id" +
                    "VALUES (@photo)");
                newPhotoId = Convert.ToInt32(cmd.ExecuteNonQuery());
            }

            return GetPhotoById(newPhotoId);
        }


        public int SaveFile(string fileData, string fileName)
        {
            try
            {
                int postId = -1;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO photos (photo_name, photo) OUTPUT inserted.photo_id VALUES (@name, @photo)", conn);
                    cmd.Parameters.AddWithValue("@name", fileName);
                    cmd.Parameters.AddWithValue("@photo", fileData);

                    postId = (int)cmd.ExecuteScalar();
                }
                return postId;
            }

            catch (Exception)
            {
                throw;
            }
        }

        public Photo GetLikesFromPost(int postId)
        {
            Photo photo = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT p.photo_id, p.photo, p.photo_name  FROM photos p JOIN posts ON posts.photo_id = p.photo_id WHERE post_id = @post_id ", conn);
                cmd.Parameters.AddWithValue("@post_id", postId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    photo = CreatePhotoFromReader(reader);
                }

            }

            return photo;

        }


        public List<Photo> GetAllPhotos()
        {
            List<Photo> photo = new List<Photo>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT photo_id, photo, photo_name FROM photos", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Photo NewPhotoList = CreatePhotoFromReader(reader);
                    photo.Add(NewPhotoList);
                }
            }

            return photo;


        }

        public Photo GetPhotoById(int Photoid)
        {
            Photo photo = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT photo_id, photo, photo_name FROM photos WHERE photo_id = @photo_id", conn);
                cmd.Parameters.AddWithValue("@photo_id", Photoid);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    photo = CreatePhotoFromReader(reader);
                }

            }
            return photo;
        }

        private Photo CreatePhotoFromReader(SqlDataReader reader)
        {
            Photo photo = new Photo();
            photo.photoId = Convert.ToInt32(reader["photo_id"]);
            photo.photo = Convert.ToString(reader["photo"]);
            photo.photoName = Convert.ToString(reader["photo_name"]);

            return photo;
        }
    }
}


