using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Evernote.Model;
using Evernote.DataLayer.Sql;
using Evernote.DataLayer;
using NLog;
using Evernote.Logger;
using Evernote.Api.Filters;
using Evernote.Api.Configurator;


namespace Evernote.Api.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        public UsersController()
        {
            string ConnectionString = GetConnectionString.GetConnectionStringByName("ConnectionString");
            _usersRepository = new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString));
            _categoriesRepository = new CategoriesRepository(ConnectionString);
        }

        /// <summary>
        /// Получить пользователя по идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <returns></returns>
        [HttpGet]
        [RepositoryExceptionFilter]
        [Route("api/users/{id}")]
        public User Get(Guid id)
        {
            return _usersRepository.Get(id);
        }

        /// <summary>
        /// Получить пользователя по имени
        /// </summary>
        /// <param name="name">имя</param>
        /// <returns></returns>
        [HttpGet]
        [RepositoryExceptionFilter]
        [Route("api/users/name/{name}")]
        public User Get(string name)
        {
            return _usersRepository.Get(name);
        }

        /// <summary>
        /// Cоздание пользователя
        /// </summary>
        /// <param name="user">пользователь</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/users")]
        public User Post([FromBody] User user)
        {
            Logger.Log.Instance.Info("Создание пользователя с именем: {0}", user.Name);

            return _usersRepository.Create(user);
        }
        /// <summary>
        /// Удаление пользователя по идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        [HttpDelete]
        [Route("api/users/{id}")]
        public void Delete(Guid id)
        {
            Logger.Log.Instance.Info("Удаление пользователя с id: {0}", id);

            _usersRepository.Delete(id);
        }

        /// <summary>
        /// Получение категорий пользователя по его идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <returns></returns>
        [HttpGet]
        [RepositoryExceptionFilter]
        [Route("api/users/{id}/categories")]
        public IEnumerable<Category> GetUserCategories(Guid id)
        {
            return _usersRepository.Get(id).Categories;

        }

        

        /// <summary>
        /// Cоздание категории
        /// </summary>
        /// <param name="userid">владелец категории</param>
        /// <param name="category">имя категории</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/users/{userid}/category")]
        public Category Post(Guid userid, [FromBody] Category category)
        {
            Logger.Log.Instance.Info("Создание категории с именем: {0}", category.Name);

            return _categoriesRepository.Create(userid, category.Name);
        }
    }
}
