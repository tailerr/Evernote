using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evernote.Model;

namespace Evernote.DataLayer
{
    public interface ICategoriesRepository
    {
        Category Create(Guid userId, string name);

        void Delete(Guid catId);

        void Update(Guid catId, string name);

        Category Get(Guid catId);

        Category Get(string catName);

        IEnumerable<Category> GetUserCategories(Guid userId);

        IEnumerable<Category> GetNoteCategories(Guid noteId);

    }
}
