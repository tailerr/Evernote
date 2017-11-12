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
    /// Логика взаимодействия для CategoryControl.xaml
    /// </summary>
    public partial class CategoryControl : UserControl
    {
        public Category category;
        public Note note;

        public CategoryControl()
        {
            InitializeComponent();
        }

        public delegate void CatChangeEventHandler(object source, CategoryEventArgs e);

        public event CatChangeEventHandler CatChanged;
        public event CatChangeEventHandler DelCatFromNoteEvent;
        public event CatChangeEventHandler CatDel;

        private void CatName_LostFocus(object sender, RoutedEventArgs e)
        {
            category.Name = CatName.Text;
            CatChanged?.Invoke(this, new CategoryEventArgs(this.category));
        }

        private void CatName_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                TextBox tb = sender as TextBox;
                ContextMenu contextMenu = tb.ContextMenu;
                contextMenu.PlacementTarget = tb;
                contextMenu.IsOpen = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            CatDel?.Invoke(this, new CategoryEventArgs(this.category));
        }

        private void DelCatFromNote_Click(object sender, RoutedEventArgs e)
        {
            DelCatFromNoteEvent?.Invoke(this, new CategoryEventArgs(this.category, this.note));
        }
    }
    public class CategoryEventArgs : EventArgs
    {
        public Note Note;
        public Category Cat;
        public CategoryEventArgs(Category cat, Note note=null)
        {
            Cat = cat;
            Note = note;
        }
    }
}
