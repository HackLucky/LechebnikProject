using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для ManageUsersWindow.xaml
    /// </summary>
    public partial class ManageUsersWindow : Window
    {
        public ManageUsersWindow()
        {
            InitializeComponent();
            DataContext = new ManageUsersViewModel();
        }
    }
}
