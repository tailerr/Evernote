using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Evernote.Model;
using System.Collections.Generic;
using System.Linq;

namespace Evernote.DataLayer.Sql.Tests
{
    [TestClass]
    public class UsersRepositoryTests
    {
        private const string ConnectionString = @"Data Source=LAPTOP-BSCP12KB\SQLEXPRESS;
                                                Database=myDb;
                                                Trusted_Connection = True";
        private readonly List<Guid> _tempUsers = new List<Guid>();
        private readonly List<Guid> _tempCategories = new List<Guid>();

        [TestMethod]
        public void ShouldCreateUser()
        {
            var user = new User
            {
                Name = "test",
                Email = "testmail"
            };

            var repository = new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString));
            var result = repository.Create(user);
            _tempUsers.Add(user.Id);

            var userFromDb = repository.Get(result.Id);

            Assert.AreEqual(result.Name, userFromDb.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldDeleteUser()
        {
            var user = new User
            {
                Name = "test",
                Email = "testmail"
            };

            var repository = new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString));
            var result = repository.Create(user);
            _tempUsers.Add(result.Id);
            repository.Delete(result.Id);
            var userFromDb=repository.Get(result.Id);
        }

        [TestMethod]
        public void ShouldCreateUserAndAddCategory()
        {
            //arrange
            var user = new User
            {
                Name = "test",
                Email = "testemail"
            };
            const string category = "testCategory";

            //act
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString, categoriesRepository);
            user = usersRepository.Create(user);

            _tempUsers.Add(user.Id);

            var resCategory = categoriesRepository.Create(user.Id, category);
            _tempCategories.Add(resCategory.Id);
            user = usersRepository.Get(user.Id);

            //asserts
            Assert.AreEqual(category, user.Categories.Single().Name);
        }


        [TestCleanup]
        public void CleanData()
        {
            foreach (var id in _tempCategories)
                new CategoriesRepository(ConnectionString).Delete(id);
            foreach (var id in _tempUsers)
                new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString)).Delete(id);
            
        }
    }
}

