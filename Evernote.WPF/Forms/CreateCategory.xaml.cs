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
using System.Windows.Shapes;

namespace Evernote.WPF.Forms
{
    /// <summary>
    /// Логика взаимодействия для CreateCategory.xaml
    /// </summary>
    public partial class CreateCategory : Window
    {
        public CreateCategory()
        {
            InitializeComponent();
        }
        public string CategoryName
        {
            get { return GetCategoryName.Text; }
        }
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

        }
    }
}
