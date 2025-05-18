using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для ContactAdminWindow.xaml
    /// </summary>
    public partial class ContactAdminWindow : Window
    {
        public ContactAdminWindow()
        {
            InitializeComponent();
            DataContext = new ContactAdminViewModel();
        }
    }
}
