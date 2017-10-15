using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Evernote.Model;
using Evernote.DataLayer.Sql;

namespace UsersRepositoryTest
{
    [TestClass]
    public class NotesRepositoryTests
    {
        private const string ConnectionString = @"Data Source=LAPTOP-BSCP12KB\SQLEXPRESS;
                                                Database=myDb;
                                                Trusted_Connection = True";
        private readonly List<Guid> _tempNotes = new List<Guid>();
        private readonly List<Guid> _tempUsers = new List<Guid>();
        private readonly List<Guid> _tempCategories = new List<Guid>();

        [TestMethod]
        public void ShouldCreateNote()
        {
            var user = new User
            {
                Name = "test",
                Email = "testmail"
            };
            var categoryRepository = new CategoriesRepository(ConnectionString);
            var userRepository = new UsersRepository(ConnectionString, categoryRepository);
            var noteRepository = new NotesRepository(ConnectionString, userRepository, categoryRepository);
            user = userRepository.Create(user);
            _tempUsers.Add(user.Id);

            var note = new Note
            {
                Head = "testHead1",
                Text = "testText1",
                Owner = user.Id,
            };
            note = noteRepository.Create(note);
            _tempNotes.Add(note.Id);
            var noteFromDb = noteRepository.Get(note.Id);

            Assert.AreEqual(note.Head, noteFromDb.Head);
            Assert.AreEqual(note.Text, noteFromDb.Text);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldDeleteNote()
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
                Head = "testHead2",
                Text = "testText2",
                Owner = userResult.Id,
            };
            
            var noteResult = noteRepository.Create(note);
            _tempNotes.Add(noteResult.Id);
            noteRepository.Delete(noteResult.Id);
            var noteFromDb = noteRepository.Get(noteResult.Id);
        }

        [TestMethod]
        public void ShouldChangeNote()
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
            noteResult.Text = noteResult.Text+"1";
            noteResult.Head = noteResult.Head+"1";
            noteRepository.ChangeNote(noteResult);
            var noteFromDb = noteRepository.Get(noteResult.Id);
            Assert.AreEqual("testText1", noteFromDb.Text);
            Assert.AreEqual("testHead1", noteFromDb.Head);
        }

        [TestMethod]
        public void ShouldGetUsersNotes()
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
            var resNote = noteRepository.Create(note);
            _tempNotes.Add(resNote.Id);
            var UsersNotes = noteRepository.GetUsersNotes(userResult.Id);
            Assert.AreEqual(resNote.Id, UsersNotes.ElementAt(0).Id);
        }
        [TestMethod]
        public void ShouldShareNote()
        {
            var user = new User
            {
                Name = "test",
                Email = "testmail"
            };
            var user1 = new User
            {
                Name = "test1",
                Email = "testmail1"
            };
            var categoryRepository = new CategoriesRepository(ConnectionString);
            var userRepository = new UsersRepository(ConnectionString, categoryRepository);
            var noteRepository = new NotesRepository(ConnectionString, userRepository, categoryRepository);

            var userResult = userRepository.Create(user);
            var userResult1 = userRepository.Create(user1);
            _tempUsers.Add(userResult.Id);
            _tempUsers.Add(userResult1.Id);
            var note = new Note
            {
                Head = "testHead",
                Text = "testText",
                Owner = userResult.Id,
            };
            var noteResult = noteRepository.Create(note);
            _tempNotes.Add(noteResult.Id);
            noteRepository.ShareNote(noteResult.Id, userResult1.Id);
            var noteFromDb = noteRepository.Get(noteResult.Id);
            Assert.AreEqual(noteFromDb.Shared.ElementAt(0).Id, userResult1.Id);

        }

        [TestMethod]
        public void ShouldAddCategoryToNote()
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
            var noteFromDb = noteRepository.Get(noteResult.Id);
            Assert.AreEqual(category, noteFromDb.Categories.ElementAt(0).Name); 
        }

        [TestMethod]
        public void ShouldDeleteCategoryfromNote()
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
            noteRepository.DeleteCategory(noteResult.Id, resCategory.Id);
            var noteFromDb = noteRepository.Get(noteResult.Id);
            Assert.AreEqual(0, noteFromDb.Categories.Count());
        }
        [TestCleanup]
        public void CleanData()
        {
            foreach (var id in _tempUsers)
                new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString)).Delete(id);
            foreach (var id in _tempNotes)
                new NotesRepository(ConnectionString,new UsersRepository(ConnectionString, new CategoriesRepository(ConnectionString)),
                    new CategoriesRepository(ConnectionString)).Delete(id);
            foreach (var id in _tempCategories)
                new CategoriesRepository(ConnectionString).Delete(id);
        }
    }
}
