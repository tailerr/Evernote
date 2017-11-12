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

namespace Evernote.WPF.Controls
{
    /// <summary>
    /// Логика взаимодействия для LoginPanel.xaml
    /// </summary>
    public partial class LoginPanel : UserControl
    {
        public LoginPanel()
        {
            InitializeComponent();
        }

        public delegate void LoginEventHandler(object source, LoginEventArgs e);

        public event LoginEventHandler LoginClick;

        public event LoginEventHandler SignUpClick;

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            LoginClick?.Invoke(this, new LoginEventArgs(this.GetUserName.Text, this.GetUserPass.Password));
        }

        private void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            SignUpClick?.Invoke(this, new LoginEventArgs(this.GetUserName.Text, this.GetUserPass.Password));
        }
    }

    public class LoginEventArgs : EventArgs
    {
        public string Login;
        public string Pass;
        public LoginEventArgs(string login, string pass)
        {
            Login = login;
            Pass = pass;
        }
    } 
}

