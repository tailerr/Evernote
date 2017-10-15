using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evernote.Model;
using System.Data.SqlClient;

namespace Evernote.DataLayer.Sql
{
    public class NotesRepository : INotesRepository
    {
        private readonly UsersRepository _usersRepository;

        private readonly CategoriesRepository _categoriesRepository;

        private readonly string _connectionString;

        public NotesRepository(string connectionString, UsersRepository uR, CategoriesRepository cR)
        {
            _connectionString = connectionString;
            _usersRepository = uR;
            _categoriesRepository = cR;
        }

        public Note Create(Note note)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"insert into Notes (id, DateCreation, Text, DateOfChange, Head, Owner) 
                        values (@id, @DateCreation, @Text, @DateOfChange, @Head, @UserId)";

                    note.Id = Guid.NewGuid();
                    command.Parameters.AddWithValue("@id", note.Id);
                    command.Parameters.AddWithValue("@DateCreation", DateTime.Now);
                    command.Parameters.AddWithValue("@DateOfChange", DateTime.Now);
                    command.Parameters.AddWithValue("@Text", note.Text);
                    command.Parameters.AddWithValue("@Head", note.Head);
                    command.Parameters.AddWithValue("@UserId", note.Owner);
                    command.ExecuteNonQuery();

                    return note;
                }
            }
        }

        public void Delete(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from Notes where id=@id";
                    command.Parameters.AddWithValue("@id", noteId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void ChangeNote(Note note)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"update Notes set Text=@Text, DateOfChange=@DateOfChange,
                                            Head=@Head where id=@id";
                    command.Parameters.AddWithValue("@Text", note.Text);
                    command.Parameters.AddWithValue("@DateOfChange", DateTime.Now);
                    command.Parameters.AddWithValue("@Head", note.Head);
                    command.Parameters.AddWithValue("@id", note.Id);
                    command.ExecuteNonQuery();
                }
            }
        }
        public IEnumerable<Note> GetUsersNotes(Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select * from Notes where Owner=@userid";
                    command.Parameters.AddWithValue("@userid", userId);

                    using(var reader = command.ExecuteReader())
                    {
                        User user = _usersRepository.Get(userId);

                        while (reader.Read())
                        {
                            using (var command1 = sqlConnection.CreateCommand())
                            {

                            }

                            yield return new Note
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                Text = reader.GetString(reader.GetOrdinal("Text")),
                                Head = reader.GetString(reader.GetOrdinal("Head")),
                                Changed = reader.GetDateTime(reader.GetOrdinal("DateOfChange")),
                                Created = reader.GetDateTime(reader.GetOrdinal("DateCreation")),
                                Owner = user.Id,
                                Categories = _categoriesRepository.GetNoteCategories(reader.GetGuid(reader.GetOrdinal("id")))
                            };
                        }
                    }
                }
            }
        }

        public void ShareNote(Guid noteId, Guid userId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "insert into Shares (NoteId, UserId) values (@NoteId, @UserId)";
                    command.Parameters.AddWithValue("@NoteId", noteId);
                    command.Parameters.AddWithValue("@UserId", userId);

                    command.ExecuteNonQuery();

                }
            }
        }

        public void AddCategory(Guid noteId, Guid catId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "insert into CategoriesToNotes (NoteId, CategoryId) values (@NoteId, @CatId)";
                    command.Parameters.AddWithValue("@NoteId", noteId);
                    command.Parameters.AddWithValue("@CatId", catId);

                    command.ExecuteNonQuery();

                }
            }
        }

        public void DeleteCategory(Guid noteId, Guid categoryId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "delete from CategoriesToNotes where NoteId=@NoteId and CategoryId=@CatId";
                    command.Parameters.AddWithValue("@NoteId", noteId);
                    command.Parameters.AddWithValue("@CatId", categoryId);

                    command.ExecuteNonQuery();

                }
            }
        }

        public Note Get(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = "select * from Notes where id=@id";
                    command.Parameters.AddWithValue("@id", noteId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new ArgumentException($"Заметка с id {noteId} не найдена");

                        var note = new Note
                        {
                            Id = noteId,
                            Head = reader.GetString(reader.GetOrdinal("Head")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            Created = reader.GetDateTime(reader.GetOrdinal("DateCreation")),
                            Changed = reader.GetDateTime(reader.GetOrdinal("DateOfChange")),
                        };
                        note.Shared = GetShares(note.Id);
                        note.Owner = _usersRepository.Get(reader.GetGuid(reader.GetOrdinal("Owner"))).Id;
                        note.Categories = _categoriesRepository.GetNoteCategories(noteId);
                        return note;
                    }
                }
            }

        }

        public IEnumerable<User> GetShares(Guid noteId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandText = @"select * from Shares right join Users on Shares.UserId=Users.id
                                           where Shares.NoteId=@noteid";
                    command.Parameters.AddWithValue("@noteid", noteId);

                    using (var reader = command.ExecuteReader())
                    {
                       while (reader.Read())
                        {
                            var user = new User
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Id = reader.GetGuid(reader.GetOrdinal("id")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                           };
                            user.Categories = _categoriesRepository.GetUserCategories(user.Id);

                            yield return user;
                        }
                    }
                }
            }
        }
    }
}
