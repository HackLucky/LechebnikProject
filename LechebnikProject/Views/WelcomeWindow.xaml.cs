using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для WelcomeWindow.xaml
    /// </summary>
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.WelcomeViewModel(); // Установка ViewModel
        }
    }
}