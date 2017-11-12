using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Evernote.Model;
using Evernote.DataLayer.Sql;
using Evernote.DataLayer;
using Evernote.Api.Filters;
using Evernote.Api.Configurator;

namespace Evernote.Api.Controllers
{
    public class NotesController : ApiController
    {
        private readonly INotesRepository _notesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        public NotesController()
        {
            string ConnectionString = GetConnectionString.GetConnectionStringByName("ConnectionString");
            _notesRepository = new NotesRepository(ConnectionString,
                new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString)), 
                new CategoriesRepository(ConnectionString));
            _usersRepository = new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString));
            _categoriesRepository = new CategoriesRepository(ConnectionString);

            
        }

        /// <summary>
        /// Получение записи по идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <returns></returns>
        [RepositoryExceptionFilter]
        [HttpGet]
        [Route("api/notes/{id}")]
        public Note Get(Guid id)
        {
            return _notesRepository.Get(id);
            
        }

        /// <summary>
        /// Создание записи
        /// </summary>
        /// <param name="note">запись</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/notes")]
        public Note Post(Note note)
        {
            Logger.Log.Instance.Info("Создание заметки с заголовком: {0}", note.Head);

            return _notesRepository.Create(note);
        }

        /// <summary>
        /// Удаление записи по идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        [HttpDelete]
        [Route("api/notes/{id}")]
        public void Delete(Guid id)
        {
            Logger.Log.Instance.Info("Удаление заметки с id: {0}", id);

            _notesRepository.Delete(id);
        }
        /// <summary>
        /// Изменение записи
        /// </summary>
        /// <param name="note">запись</param>
        [HttpPut]
        [RepositoryExceptionFilter]
        [Route("api/notes")]
        public void Update(Note note)
        {
            Logger.Log.Instance.Info("Изменение заметки с id: {0}", note.Id);

            _notesRepository.ChangeNote(note);
        }

        /// <summary>
        /// Получение заметок пользователя по его идентификатору
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/users/{userId}/notes")]
        [RepositoryExceptionFilter]
        public IEnumerable<Note> GetUsersNotes(Guid userId)
        {
            return _notesRepository.GetUsersNotes(userId);
        }

        /// <summary>
        /// Получение расшареных заметок пользователя по его идентификатору
        /// </summary>
        /// <param name="userId">идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/users/{userId}/sharednotes")]
        [RepositoryExceptionFilter]
        public IEnumerable<Note> GetSharedNotes(Guid userId)
        {
            return _notesRepository.GetSharedNotes(userId);
        }

        /// <summary>
        /// Получение категорий заметки по ее идентификатору
        /// </summary>
        /// <param name="noteId">идентификатор заметки</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/notes/{noteId}/categories")]
        [RepositoryExceptionFilter]
        public IEnumerable<Category> GetNoteCategories(Guid noteId)
        {
            return _categoriesRepository.GetNoteCategories(noteId);
        }

        /// <summary>
        /// Поделиться записью с пользователем по идентификаторам
        /// </summary>
        /// <param name="noteId">id заметки</param>
        /// <param name="userId">id пользователя</param>
        [HttpPost]
        [RepositoryExceptionFilter]
        [Route("api/users/{userId}/share/{noteId}")]
        public void Share(Guid noteId, Guid userId)
        {
            Logger.Log.Instance.Info("Sharing заметки с id: {0} пользователю {1}", noteId, userId);
            _notesRepository.ShareNote(noteId, userId);
        }

        /// <summary>
        /// Узнать с кем поделились записью
        /// </summary>
        /// <param name="noteId">идентификатор</param>
        /// <returns></returns>
        [HttpGet]
        [RepositoryExceptionFilter]
        [Route("api/users/{noteId}/shares")]
        public IEnumerable<User> GetShares(Guid noteId)
        {
            return _notesRepository.GetShares(noteId);
        }

        /// <summary>
        /// Добавить категорию к заметке
        /// </summary>
        /// <param name="noteId">id заметки</param>
        /// <param name="catId">id категории</param>
        [HttpPost]
        [RepositoryExceptionFilter]
        [Route("api/notes/{noteId}/categories/{catId}")]
        public void AddCategory(Guid noteId, Guid catId)
        {
            Logger.Log.Instance.Info("Добавление категории {0} к заметке {1}",catId, noteId);
            _notesRepository.AddCategory(noteId, catId);
        }
        /// <summary>
        /// Удалить категорию у заметки
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="catId"></param>
        [HttpDelete]
        [RepositoryExceptionFilter]
        [Route("api/notes/{noteId}/categories/{catId}")]
        public void DeleteCategory(Guid noteId, Guid catId)
        {
            Logger.Log.Instance.Info("Удаление категории {0} у заметки {1}", catId, noteId);
            _notesRepository.DeleteCategory(noteId, catId);
        }
    }
}
