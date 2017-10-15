using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evernote.Model
{
    public class Note
    {
        public Guid Id { get; set; }

        public string Head { get; set; }

        public string Text { get; set; }

        public DateTime Created { get; set; }

        public DateTime Changed { get; set; }

        public Guid Owner { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<User> Shared { get; set; }
    }
}
