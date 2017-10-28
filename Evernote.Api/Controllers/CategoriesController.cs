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
    public class CategoriesController : ApiController
    {
        private const string ConnectionString = @"Data Source=LAPTOP-BSCP12KB\SQLEXPRESS;
                                                Database=myDb;
                                                Trusted_Connection = True";
        private readonly ICategoriesRepository _categoriesRepository;

        public CategoriesController()
        {
            _categoriesRepository = new CategoriesRepository(ConnectionString);
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
        [Route("api/categories")]
        public void Update(Category category)
        {
            var catFromDb = _categoriesRepository.Get(category.Id);
            if (catFromDb == null)
            {
                Logger.Log.Instance.Error("Категории с идентификатором: {0} не существует", category.Id);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Категории с идентификатором: {0} не существует", category.Id)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
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
        public Category Get(Guid id)
        {
            var categrory = _categoriesRepository.Get(id);
            if (categrory == null)
            {
                Logger.Log.Instance.Error("Категории с идентификатором {0} не существует", id);
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Format("Категории с идентификатором: {0} не существует", id)),
                    ReasonPhrase = "Product ID Not Found"
                };
                throw new HttpResponseException(resp);
            }
            return categrory;
        }


    }
}
