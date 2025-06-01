using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ContactAdminWindow : Window
    {
        public ContactAdminWindow()
        {
            InitializeComponent();
            DataContext = new ContactAdminViewModel();
        }
    }
}
