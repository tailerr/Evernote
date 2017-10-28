using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evernote.Model;
using System.Data.SqlClient;

namespace Evernote.DataLayer.Sql
{
    public class UsersRepository : IUsersRepository
    {
        private readonly string _connectionString;

        private readonly ICategoriesRepository _categoriesRepository;
        
        public UsersRepository(string connectionString, ICategoriesRepository categoriesRepository)
        {
            _connectionString = connectionString;
            _categoriesRepository = categoriesRepository;
        }

        public User Create(User user)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "insert into Users (id, Email, Name) values (@id, @Email, @Name)";

                    user.Id = Guid.NewGuid();

                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Name", user.Name);
                    command.ExecuteNonQuery();

                    return user;
                }
            }
        }

        public User Get(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select * from Users where id=@id";
                    command.Parameters.AddWithValue("@id", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        var user = new User
                        {
                            Id = userId,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email"))
                        };
                        user.Categories = _categoriesRepository.GetUserCategories(userId);
                        return user;
                    }
                }
            }

        }

        public void Delete(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from Shares where UserId=@id";
                    command.Parameters.AddWithValue("@id", userId);
                    command.ExecuteNonQuery();
                }

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from Users where id=@id";
                    command.Parameters.AddWithValue("@id", userId);
                    command.ExecuteNonQuery();
                }
            }
        }


    }
}
