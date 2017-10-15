using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evernote.Model;
using System.Data.SqlClient;


namespace Evernote.DataLayer.Sql
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly string _connectionString;

        public CategoriesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Category Create(Guid userId, string name)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using(var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "insert into Categories (id, UserId, Name) values (@id, @UserId, @Name)";

                    var category = new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = name
                    };
                    

                    command.Parameters.AddWithValue("@id", category.Id);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Name", category.Name);

                    command.ExecuteNonQuery();

                    return category;
                }
            }
        }

        public IEnumerable<Category> GetNoteCategories(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"select * from CategoriesToNotes right join Categories on CategoriesToNotes.CategoryId=Categories.id
                                           where CategoriesToNotes.NoteId=@noteid";
                    command.Parameters.AddWithValue("@noteid", noteId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Category
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Id = reader.GetGuid(reader.GetOrdinal("id"))
                            };
                        }
                    }
                }
            }
        }

        public IEnumerable<Category> GetUserCategories(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select id, Name from Categories where UserId=@userid";
                    command.Parameters.AddWithValue("@userid", userId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return new Category
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Id = reader.GetGuid(reader.GetOrdinal("id"))
                            };
                        }
                    }
                }
            }
        }

        public void Delete(Guid catId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from Categories where id=@id";
                    command.Parameters.AddWithValue("@id", catId);
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    
}
