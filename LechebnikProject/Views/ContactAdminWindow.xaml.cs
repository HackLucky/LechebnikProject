using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ContactAdminWindow : Window
    {
        public ContactAdminWindow()
        {
            InitializeComponent();
            MessageTextBox.Focus();
            DataContext = new ContactAdminViewModel();
        }
    }
}
