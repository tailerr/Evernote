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
        private readonly IUsersRepository _usersRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        public NotesController()
        {
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
        [HttpGet]
        [Route("api/notes/{id}")]
        public Note Get(Guid id)
        {
            var note = _notesRepository.Get(id);
            if (note == null)
            {
                Logger.Log.Instance.Error("Запись с идентификатором {0} не существует", id);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Заметки с идентификатором: {0} не существует", note.Id)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
            return note;
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
        [Route("api/notes")]
        public void Update(Note note)
        {
            var notefromdb = _notesRepository.Get(note.Id);
            if (notefromdb == null)
            {
                Logger.Log.Instance.Error("Заметки с идентификатором: {0} не существует", note.Id);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Заметки с идентификатором: {0} не существует", note.Id)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
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
        public IEnumerable<Note> GetUsersNotes(Guid userId)
        {
            var user = _usersRepository.Get(userId);
            if (user == null)
            {
                Logger.Log.Instance.Error("Пользователя с идентификатором: {0} не существует", userId);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Пользователя с идентификатором: {0} не существует", userId)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
            return _notesRepository.GetUsersNotes(userId);
        }

        /// <summary>
        /// Поделиться записью с пользователем по идентификаторам
        /// </summary>
        /// <param name="noteId">id заметки</param>
        /// <param name="userId">id пользователя</param>
        [HttpPost]
        [Route("api/users/{userId}/share/{noteId}")]
        public void Share(Guid noteId, Guid userId)
        {
            var note = _notesRepository.Get(noteId);
            var user = _usersRepository.Get(userId);
            if (user == null)
            {
                Logger.Log.Instance.Error("Пользователя с идентификатором: {0} не существует", userId);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Пользователя с идентификатором: {0} не существует", userId)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
            if (note == null)
            {
                Logger.Log.Instance.Error("Запись с идентификатором {0} не существует", noteId);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Заметки с идентификатором: {0} не существует", note.Id)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
            Logger.Log.Instance.Info("Sharing заметки с id: {0} пользователю {1}", noteId, userId);
            _notesRepository.ShareNote(noteId, userId);
        }

        /// <summary>
        /// Узнать с кем поделились записью
        /// </summary>
        /// <param name="noteId">идентификатор</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/users/{noteId}/shares")]
        public IEnumerable<User> GetShares(Guid noteId)
        {
            var note = _notesRepository.Get(noteId);
            if (note == null)
            {
                Logger.Log.Instance.Error("Запись с идентификатором {0} не существует", noteId);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Заметки с идентификатором: {0} не существует", note.Id)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
            return _notesRepository.GetShares(noteId);
        }

        /// <summary>
        /// Добавить категорию к заметке
        /// </summary>
        /// <param name="noteId">id заметки</param>
        /// <param name="catId">id категории</param>
        [HttpPost]
        [Route("api/notes/{noteId}/categories/{catId}")]
        public void AddCategory(Guid noteId, Guid catId)
        {
            var categrory = _categoriesRepository.Get(catId);
            if (categrory == null)
            {
                Logger.Log.Instance.Error("Категории с идентификатором {0} не существует", catId);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Категории с идентификатором: {0} не существует", catId)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
            var note = _notesRepository.Get(noteId);
            if (note == null)
            {
                Logger.Log.Instance.Error("Запись с идентификатором {0} не существует", noteId);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Заметки с идентификатором: {0} не существует", note.Id)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
            Logger.Log.Instance.Info("Добавление категории {0} к заметке {1}",catId, noteId);
            _notesRepository.AddCategory(noteId, catId);
        }

        [HttpDelete]
        [Route("api/notes/{noteId}/categories/{catId}")]
        public void DeleteCategory(Guid noteId, Guid catId)
        {
            Logger.Log.Instance.Info("Удаление категории {0} у заметки {1}", catId, noteId);
            _notesRepository.DeleteCategory(noteId, catId);
        }
    }
}
