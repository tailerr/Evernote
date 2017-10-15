using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evernote.Model;

namespace Evernote.DataLayer
{
    public interface INotesRepository
    {
        Note Create(Note note);

        Note Get(Guid noteId);

        void Delete(Guid noteId);

        void ChangeNote(Note note);

        IEnumerable<Note> GetUsersNotes(Guid userId);

        void ShareNote(Guid noteId, Guid userId);

        IEnumerable<User> GetShares(Guid noteId);

        void AddCategory(Guid noteId, Guid catId);

        void DeleteCategory(Guid noteId, Guid categoryId);

        
    }
}
