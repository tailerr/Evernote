using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Evernote.Model;
using Evernote.DataLayer.Sql;
using Evernote.DataLayer;

namespace UsersRepositoryTest
{
    [TestClass]
    public class CategoriesRepositoryTests
    {
        private const string ConnectionString = @"Data Source=LAPTOP-BSCP12KB\SQLEXPRESS;
                                                Database=myDb;
                                                Trusted_Connection = True";
        private readonly List<Guid> _tempUsers = new List<Guid>();
        private readonly List<Guid> _tempNotes = new List<Guid>();
        private readonly List<Guid> _tempCategories = new List<Guid>();

        [TestMethod]
        public void ShouldAddCategory()
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

        [TestMethod]
        public void ShouldDeleteCategory()
        {
            var user = new User
            {
                Name = "test",
                Email = "testmail"
            };
            const string category = "testcategory";
            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString, categoriesRepository);
            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);

            var resultCat = categoriesRepository.Create(user.Id, category);
            _tempCategories.Add(resultCat.Id);
            categoriesRepository.Delete(resultCat.Id);
            var userFromDb = usersRepository.Get(user.Id);
            Assert.IsTrue(!userFromDb.Categories.Any());
        }

        [TestMethod]
        public void ShouldGetUsersCategories()
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

            var cat = categoriesRepository.Create(user.Id, category);
            _tempCategories.Add(cat.Id);
            var resFromDb = categoriesRepository.GetUserCategories(user.Id);

            //asserts 
            Assert.AreEqual(category, resFromDb.ElementAt(0).Name);
        }

        [TestMethod]
        public void ShouldGetNoteCategories()
        {
            var user = new User
            {
                Name = "test",
                Email = "testmail"
            };
            var categoryRepository = new CategoriesRepository(ConnectionString);
            var userRepository = new UsersRepository(ConnectionString, categoryRepository);
            var noteRepository = new NotesRepository(ConnectionString, userRepository, categoryRepository);

            var userResult = userRepository.Create(user);
            _tempUsers.Add(userResult.Id);
            var note = new Note
            {
                Head = "testHead",
                Text = "testText",
                Owner = userResult.Id,
            };
            var noteResult = noteRepository.Create(note);
            _tempNotes.Add(noteResult.Id);
            const string category = "category";
            var resCategory = categoryRepository.Create(userResult.Id, category);
            _tempCategories.Add(resCategory.Id);
            noteRepository.AddCategory(noteResult.Id, resCategory.Id);
            var result = categoryRepository.GetNoteCategories(noteResult.Id);
            Assert.AreEqual(result.ElementAt(0).Id, resCategory.Id);


        }

        [TestMethod]
        public void ShouldUpdateCategory()
        {
            var user = new User
            {
                Name = "test",
                Email = "testemail"
            };
            const string category = "testCategory";
            const string newcategory = "newtestCategory";


            var categoriesRepository = new CategoriesRepository(ConnectionString);
            var usersRepository = new UsersRepository(ConnectionString, categoriesRepository);
            user = usersRepository.Create(user);
            _tempUsers.Add(user.Id);

            var cat = categoriesRepository.Create(user.Id, category);
            _tempCategories.Add(cat.Id);
            categoriesRepository.Update(cat.Id, newcategory);
            var catFromDb = categoriesRepository.GetUserCategories(user.Id);

            Assert.AreEqual(newcategory, catFromDb.ElementAt(0).Name);

        }

        [TestCleanup]
        public void CleanData()
        {
            foreach (var id in _tempCategories)
                new CategoriesRepository(ConnectionString).Delete(id);
            foreach (var id in _tempNotes)
                new NotesRepository(ConnectionString, new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString)),
                    new CategoriesRepository(ConnectionString)).Delete(id);

            foreach (var id in _tempUsers)
                new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString)).Delete(id);
        }
    }
}