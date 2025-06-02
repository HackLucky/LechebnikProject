using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ClientLoginWindow : Window
    {
        private readonly ClientLoginViewModel _viewModel;
        public Client AuthenticatedClient { get; private set; }

        public ClientLoginWindow()
        {
            InitializeComponent();
            _viewModel = new ClientLoginViewModel();
            DataContext = _viewModel;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Authenticate())
            {
                DialogResult = true;
            }            
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowWindow<ClientRegisterWindow>();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}