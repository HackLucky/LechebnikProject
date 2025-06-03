using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
            LastNameTextBox.Focus();
            DataContext = new ProfileViewModel();
        }
    }
}
