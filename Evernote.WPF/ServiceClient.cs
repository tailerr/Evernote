using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Evernote.Model;
using System.Collections;
using System.Collections.Generic;

namespace Evernote.WPF
{
    internal class ServiceClient
    {
        private readonly HttpClient _client;

        public ServiceClient(string connectionString)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(connectionString);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public User CreateUser(User user)
        {
            user = _client.PostAsJsonAsync<User>("users", user).Result.Content.ReadAsAsync<User>().Result;
            return user;

        }
        public User GetUser(Guid id)
        {
            var user = _client.GetAsync("users/" + Convert.ToString(id)).Result.Content.ReadAsAsync<User>().Result;
            return user;
        }
        public User GetUser(string name)
        {
            var user = _client.GetAsync("users/name/" + name).Result.Content.ReadAsAsync<User>().Result;
            return user;
        }
        public void DeleteUser(Guid userId)
        {
            _client.DeleteAsync("users/" + Convert.ToString(userId));
        }
        public bool AssignUser(string userName)
        {
            return _client.GetAsync("users/name/" + userName).Result.IsSuccessStatusCode;
        }
        public List<Note> GetUserNotes(Guid userId)
        {
            var res = _client.GetAsync("users/" + Convert.ToString(userId) + "/notes").Result.Content.ReadAsAsync<List<Note>>().Result;
            return res;
        }
        public List<Category> GetUserCategories(Guid userId)
        {
            return _client.GetAsync("users/" + Convert.ToString(userId) + "/categories").Result.Content.ReadAsAsync<List<Category>>().Result;
        }

        public Category CreateCategory(Guid userId, Category category)
        {
            category = _client.PostAsJsonAsync<Category>("users/" + Convert.ToString(userId) + "/category", category).Result.Content.ReadAsAsync<Category>().Result;
            return category;
        }
        public Category GetCategory(Guid id)
        {
            var cat = _client.GetAsync("categories/" + Convert.ToString(id)).Result.Content.ReadAsAsync<Category>().Result;
            return cat;
        }
        public Category GetCategory(string catName)
        {
            var cat = _client.GetAsync("categories/name/" + catName).Result.Content.ReadAsAsync<Category>().Result;
            return cat;
        }
        public void UpdateCategory(Category cat)
        {
            _client.PutAsJsonAsync("categories", cat);
        }
        public void DeleteCategory(Guid catId)
        {
            _client.DeleteAsync("categories/" + Convert.ToString(catId));
        }
        public Note CreateNote(Note note)
        {
            note = _client.PostAsJsonAsync("notes", note).Result.Content.ReadAsAsync<Note>().Result;
            return note;
        }
        public Note GetNote(Guid id)
        {
            var note = _client.GetAsync("notes/" + Convert.ToString(id)).Result.Content.ReadAsAsync<Note>().Result;
            return note;
        }
        public void DeleteNote(Guid noteId)
        {
            _client.DeleteAsync("notes/" + Convert.ToString(noteId));
        }
        public void UpdateNote(Note note)
        {
            _client.PutAsJsonAsync("notes", note);

        }
        public bool AssignCategory(string catName)
        {
            return _client.GetAsync("categories/name/" + catName).Result.IsSuccessStatusCode;
        }
        public List<Category> GetNotesCats(Guid noteId)
        {
            return _client.GetAsync("notes/"+Convert.ToString(noteId)+"/categories").Result.Content.ReadAsAsync<List<Category>>().Result;
        }
        public void AddCatToNote(Guid noteId, Guid catId)
        {
            _client.PostAsync("notes/" + Convert.ToString(noteId) + "/categories/" + Convert.ToString(catId), null);
        }
        public void DelCatFromNote(Guid catId, Guid noteId)
        {
            _client.DeleteAsync("notes/" + Convert.ToString(noteId) + "/categories/" + Convert.ToString(catId));
        }
        public List<User> GetShares(Guid noteId)
        {
            return _client.GetAsync("users/" + Convert.ToString(noteId) + "/shares").Result.Content.ReadAsAsync<List<User>>().Result;
        }
        public void ShareNote(Guid noteId, Guid userId)
        {
            _client.PostAsync("users/" + Convert.ToString(userId) + "/share/" + Convert.ToString(noteId), null);
        }
        public List<Note> GetSharedNotes(Guid userId)
        {
            return _client.GetAsync("users/" + Convert.ToString(userId) + "/sharednotes").Result.Content.ReadAsAsync<List<Note>>().Result;

        }
    }
}
