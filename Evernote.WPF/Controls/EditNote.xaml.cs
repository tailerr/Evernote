using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Evernote.Model;

namespace Evernote.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для EditNote.xaml
    /// </summary>
    public partial class EditNote : UserControl
    {
        public Note note;
        public EditNote()
        {
            InitializeComponent();
        }

        public delegate void NoteChangeEventHandler(object source, NoteEventArgs e);

        public event NoteChangeEventHandler NoteChanged;
        public event NoteChangeEventHandler NoteDel;
        public event NoteChangeEventHandler NoteAddCat;
        public event NoteChangeEventHandler GetSharesEvent;
        public event NoteChangeEventHandler ShareNoteEvent;

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            note.Text = Body.Text;
            note.Head = Head.Text;
            NoteChanged?.Invoke(this, new NoteEventArgs(this.note));
        }

        private void MenuItemDel_Click(object sender, RoutedEventArgs e)
        {
            NoteDel?.Invoke(this, new NoteEventArgs(this.note));
        }

        private void Body_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                Grid grid = sender as Grid;
                ContextMenu contextMenu = grid.ContextMenu;
                contextMenu.PlacementTarget = grid;
                contextMenu.IsOpen = true;
            }
        }

        private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
        {
            NoteAddCat?.Invoke(this, new NoteEventArgs(this.note));
        }

        

        private void GetShares_Click(object sender, RoutedEventArgs e)
        {
            GetSharesEvent?.Invoke(this, new NoteEventArgs(this.note));
        }

        private void MenuItemShare_Click(object sender, RoutedEventArgs e)
        {
            ShareNoteEvent?.Invoke(this, new NoteEventArgs(this.note));
        }
    }

    public class NoteEventArgs : EventArgs
    {
        public Note Note;
        public NoteEventArgs(Note note)
        {
            Note = note;
        }
    }
}
   