using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Evernote.Model;
using Evernote.DataLayer.Sql;
using Evernote.DataLayer;

namespace Evernote.Api.Controllers
{
    public class NotesController : ApiController
    {
        private const string ConnectionString = @"Data Source=LAPTOP-BSCP12KB\SQLEXPRESS;
                                                Database=myDb;
                                                Trusted_Connection = True";
        private readonly INotesRepository _notesRepository;

        public NotesController()
        {
            _notesRepository = new NotesRepository(ConnectionString,
                new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString)), 
                new CategoriesRepository(ConnectionString));
        }

        [HttpGet]
        [Route("api/notes/{id}")]
        public Note Get(Guid id)
        {
            return _notesRepository.Get(id);
        }

        [HttpPost]
        [Route("api/notes")]
        public Note Post(Note note)
        {
            return _notesRepository.Create(note);
        }

        [HttpDelete]
        [Route("api/notes/{id}")]
        public void Delete(Guid id)
        {
            _notesRepository.Delete(id);
        }

        [HttpPut]
        [Route("api/notes")]
        public void Update(Note note)
        {
            _notesRepository.ChangeNote(note);
        }

        [HttpGet]
        [Route("api/users/{userId}/notes")]
        public IEnumerable<Note> GetUsersNotes(Guid userId)
        {
            return _notesRepository.GetUsersNotes(userId);
        }

        [HttpPost]
        [Route("api/users/{userId}/share/{noteId}")]
        public void Share(Guid noteId, Guid userId)
        {
            _notesRepository.ShareNote(noteId, userId);
        }

        [HttpGet]
        [Route("api/users/{noteId}/shares")]
        public IEnumerable<User> GetShares(Guid noteId)
        {
            return _notesRepository.GetShares(noteId);
        }

        [HttpPost]
        [Route("api/notes/{noteId}/categories/{catId}")]
        public void AddCategory(Guid noteId, Guid catId)
        {
            _notesRepository.AddCategory(noteId, catId);
        }

        [HttpDelete]
        [Route("api/notes/{noteId}/categories/{catId}")]
        public void DeleteCategory(Guid noteId, Guid catId)
        {
            _notesRepository.DeleteCategory(noteId, catId);
        }
    }
}
