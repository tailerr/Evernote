using System;
using System.Windows;
using Evernote.WPF.Forms;
using Evernote.WPF.Controls;
using System.Windows.Controls;
using Evernote.Model;
using System.Drawing;

namespace Evernote.WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServiceClient _serviceClient;
        private User _user;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void LogPan_SignUpClick(object source, LoginEventArgs e)
        {
            if (_serviceClient.AssignUser(e.Login))
            {
                var window = new ErrorWindow();
                window.ErrorText.Content = "Пользователь с таким именем уже существует";
                window.ShowDialog();
            }
            else
            {
                var user = _serviceClient.CreateUser(new User { Name = e.Login, Email = "" });
                _user = user;
                UserName.Content = e.Login;
                MainPage.Visibility = Visibility.Visible;
                LogPan.Visibility = Visibility.Collapsed;
            }
            
        }
        private void DelUser_Click(object sender, RoutedEventArgs e)
        {
            var window = new DeleteUserWindow();
            if (window.ShowDialog() == true)
            {
                _serviceClient.DeleteUser(_user.Id);
                UserName.Content = "";
                MainPage.Visibility = Visibility.Collapsed;
                Notes.Children.Clear();
                Categories.Children.Clear();
                LogPan.Visibility = Visibility.Visible;
                LogPan.GetUserName.Text = "";
                LogPan.GetUserPass.Password = "";
            }
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            UserName.Content = "";
            Notes.Children.Clear();
            Categories.Children.Clear();
            MainPage.Visibility = Visibility.Collapsed;
            LogPan.Visibility = Visibility.Visible;
            LogPan.GetUserName.Text = "";
            LogPan.GetUserPass.Password = "";
        }
        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {

            _serviceClient = new ServiceClient("http://localhost:59134/api/");
        }
        private void LogPan_LoginClick(object source, LoginEventArgs e)
        {
            if (_serviceClient.AssignUser(e.Login))
            {
                var user = _serviceClient.GetUser(e.Login);
                _user = user;
                UserName.Content = e.Login;
                MainPage.Visibility = Visibility.Visible;
                LogPan.Visibility = Visibility.Collapsed;
                Update();
            }
            else
            {
                LogPan.GetUserName.Text = "";
                LogPan.GetUserPass.Password = "";
                var error = new ErrorWindow();
                error.ErrorText.Content = "Не существует такого пользователя";
                error.ShowDialog();
            }
        }
        private void Update()
        {
            Notes.Children.Clear();
            Categories.Children.Clear();
            var notes = _serviceClient.GetUserNotes(_user.Id);
            notes.AddRange(_serviceClient.GetSharedNotes(_user.Id));
            for (int i = 0; i < notes.Count; i++)
            {
                var note = new EditNote();
                note.note = notes[i];
                note.Head.Text = notes[i].Head;
                note.Body.Text = notes[i].Text;
                note.NoteChanged += Note_NoteChanged;
                note.NoteDel += Note_NoteDel;
                note.NoteAddCat += Note_NoteAddCat;
                note.GetSharesEvent += Note_GetSharesEvent;
                note.ShareNoteEvent += Note_ShareNoteEvent;
                Notes.Children.Add(note);
                var noteCats = _serviceClient.GetNotesCats(notes[i].Id);
                for (int j = 0; j < noteCats.Count; j++)
                {
                    var cat = new CategoryControl();
                    cat.category = noteCats[j];
                    cat.CatName.Text = noteCats[j].Name;
                    cat.note = notes[i];
                    cat.DelCatFromNote.IsEnabled = true;
                    cat.DelCatFromNoteEvent += Cat_DelCatFromNoteEvent;
                    note.Cats.Children.Add(cat);
                }
            }
            
            var cats = _serviceClient.GetUserCategories(_user.Id);
            for (int i = 0; i < cats.Count; i++)
            {
                var cat = new CategoryControl();
                cat.category = cats[i];
                cat.CatName.Text = cats[i].Name;
                cat.CatDel += Cat_CatDel;
                cat.CatChanged += Cat_CatChanged;
                Categories.Children.Add(cat);
            }
        }

        private void Note_ShareNoteEvent(object source, NoteEventArgs e)
        {
            var window = new ShareNoteWindow();
            if (window.ShowDialog() == true)
            {
                if (_serviceClient.AssignUser(window.UserName.Text))
                {
                    var user = _serviceClient.GetUser(window.UserName.Text);
                    _serviceClient.ShareNote(e.Note.Id, user.Id);
                }
                else
                {
                    var error = new ErrorWindow();
                    error.ErrorText.Content = "Такого пользователя не существует";
                    error.ShowDialog();
                }
            }
        }

        private void Note_GetSharesEvent(object source, NoteEventArgs e)
        {
            var Shares = _serviceClient.GetShares(e.Note.Id);
            var window = new GetSharesWindow();
            for (int i = 0; i < Shares.Count; i++)
            {
                Label label = new Label();
                label.Content = Shares[i].Name;
                window.Shares.Children.Add(label);
            }
            window.ShowDialog();
            
            
        }

        private void Cat_DelCatFromNoteEvent(object source, CategoryEventArgs e)
        {
            _serviceClient.DelCatFromNote(e.Cat.Id, e.Note.Id);
            Update();
        }

        private void Note_NoteAddCat(object source, NoteEventArgs e)
        {
            var window = new AddCatToNoteWindow();
            if (window.ShowDialog() == true)
            {
                Category cat = new Category { Name = window.CatName.Text };
                if (_serviceClient.AssignCategory(window.CatName.Text))
                {
                    var category = _serviceClient.GetCategory(window.CatName.Text);
                    _serviceClient.AddCatToNote(e.Note.Id, category.Id);

                }
                else
                {
                    var category = _serviceClient.CreateCategory(_user.Id, cat);
                    _serviceClient.AddCatToNote(e.Note.Id, category.Id);
                }
                Update();
            }
        }

        private void Cat_CatDel(object source, CategoryEventArgs e)
        {
            _serviceClient.DeleteCategory(e.Cat.Id);
            Update();
        }

        private void Note_NoteDel(object source, NoteEventArgs e)
        {
            _serviceClient.DeleteNote(e.Note.Id);
            Update();

        }

        private void Note_NoteChanged(object source, NoteEventArgs e)
        {
            _serviceClient.UpdateNote(e.Note);
        }

        private void CreateCategory_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateCategory();

            if (window.ShowDialog() == true)
            {
                var category = _serviceClient.CreateCategory( _user.Id, new Category { Name = window.CategoryName });
                var cat = new CategoryControl();
                cat.CatChanged += Cat_CatChanged;
                cat.CatDel += Cat_CatDel;
                cat.category = category;
                cat.CatName.Text = category.Name;
                Categories.Children.Add(cat);
            }
        }

        private void Cat_CatChanged(object source, CategoryEventArgs e)
        {
            _serviceClient.UpdateCategory(e.Cat);
            
            
        }

        private void CreateNote_Click(object sender, RoutedEventArgs e)
        {
            var window = new CreateNoteWindow();

            if (window.ShowDialog() == true)
            {

                var note = _serviceClient.CreateNote(new Note { Owner=_user.Id, Head = window.GetNoteHead.Text, Text = window.GetNoteText.Text });
                Update();
            }
        }

        

       

        

        

        
    }
}
