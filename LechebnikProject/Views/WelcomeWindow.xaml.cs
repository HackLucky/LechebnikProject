using System.Windows;

namespace LechebnikProject.Views
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.WelcomeViewModel();
        }
    }
}