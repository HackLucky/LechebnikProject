using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            DataContext = new ProfileViewModel();
        }
    }
}
