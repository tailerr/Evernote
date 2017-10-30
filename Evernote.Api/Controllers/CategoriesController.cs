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
    public class CategoriesController : ApiController
    {
        //private const string ConnectionString = @"Data Source=LAPTOP-BSCP12KB\SQLEXPRESS;
        //                                      Database=myDb;
        //                                    Trusted_Connection = True";

        private readonly ICategoriesRepository _categoriesRepository;

        public CategoriesController()
        {
            _categoriesRepository = new CategoriesRepository(GetConnectionString.GetConnectionStringByName("ConnectionString"));
        }

        /// <summary>
        /// Cоздание категории
        /// </summary>
        /// <param name="userid">владелец категории</param>
        /// <param name="category">имя категории</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/users/{userid}/categories/{category}")] 
        public Category Post(Guid userid, string category)
        {
            Logger.Log.Instance.Info("Создание категории с именем: {0}", category);

            return _categoriesRepository.Create(userid, category);
        }

        /// <summary>
        /// Удаление категории по идентификатору
        /// </summary>
        /// <param name="categoryId">идентификатор</param>
        [HttpDelete]
        [Route("api/categories/{categoryId}")]
        public void Delete(Guid categoryId)
        {
            Logger.Log.Instance.Info("Удаление категории: {0}", categoryId);
            _categoriesRepository.Delete(categoryId);
        }

        /// <summary>
        /// Изменение категории
        /// </summary>
        /// <param name="category">категория</param>
        [HttpPut]
        [RepositoryExceptionFilter]
        [Route("api/categories")]
        public void Update(Category category)
        {
            Logger.Log.Instance.Info("Изменение категории: {0}", category.Id);
            _categoriesRepository.Update(category.Id, category.Name);
        }

        /// <summary>
        /// Получение категории по идентификатору
        /// </summary>
        /// <param name="id">идентификатор</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/categories/{id}")]
        [RepositoryExceptionFilter]
        public Category Get(Guid id)
        {
            return _categoriesRepository.Get(id);
        }


    }
}
