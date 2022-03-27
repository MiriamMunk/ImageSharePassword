using System;
using System.Data.SqlClient;

namespace FileUpload.Data
{
    public class ImageRepository
    {
        private string _ConnString;
        public ImageRepository(string conn)
        {
            _ConnString = conn;
        }
        public int AddImage(Image i)
        {
            using SqlConnection conn = new(_ConnString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT into image
                                VALUES(@path, @password, 1)SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@path", i.Path);
            cmd.Parameters.AddWithValue("@password", i.Password);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }
        public void AddView(int id)
        {
            using SqlConnection conn = new(_ConnString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Image SET Views = Views + 1 WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
        public Image GetImageById(int id)
        {
            using SqlConnection conn = new(_ConnString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Image WHERE id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return (new Image
                {
                    Id = (int)reader["Id"],
                    Password = (string)reader["password"],
                    Path = (string)reader["FilePath"],
                    NumOfViews = (int)reader["Views"]
                });
            }
            return null;
        }
        public Image VerifyPassword(int id, string password)
        {
            using SqlConnection conn = new(_ConnString);
            using SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Image WHERE id = @id AND Password = @password";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@password", password);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                AddView(id);
                return (new Image
                {
                    Id = (int)reader["id"],
                    Password = (string)reader["password"],
                    Path = (string)reader["FilePath"],
                    NumOfViews = (int)reader["Views"],
                });
            }
            return null;
        }
    }
}
