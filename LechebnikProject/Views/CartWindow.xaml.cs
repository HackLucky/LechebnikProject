using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
            SearchTextBox.Focus();
            DataContext = new CartViewModel();
        }
    }
}
