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

        private readonly ICategoriesRepository _categoriesRepository;

        public CategoriesController()
        {
            _categoriesRepository = new CategoriesRepository(GetConnectionString.GetConnectionStringByName("ConnectionString"));
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

        /// <summary>
        /// Получение категории по имени
        /// </summary>
        /// <param name="catName">имя категории</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/categories/name/{name}")]
        [RepositoryExceptionFilter]
        public Category Get(string name)
        {
            return _categoriesRepository.Get(name);
        }


    }
}
