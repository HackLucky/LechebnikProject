using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ManageUsersWindow : Window
    {
        public ManageUsersWindow()
        {
            InitializeComponent();
            SearchTextBox.Focus();
            DataContext = new ManageUsersViewModel();
        }
    }
}
