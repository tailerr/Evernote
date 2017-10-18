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

        [HttpPost]
        [Route("api/users/{userid}/categories/{category}")] 
        public Category Post(Guid userid, string category)
        {
            return _categoriesRepository.Create(userid, category);
        }

        [HttpDelete]
        [Route("api/categories/{categoryId}")]
        public void Delete(Guid categoryId)
        {
            _categoriesRepository.Delete(categoryId);
        }

        [HttpPut]
        [Route("api/categories")]
        public void Update(Category category)
        {
            _categoriesRepository.Update(category.Id, category.Name);
        }
        

    }
}
