using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для CartWindow.xaml
    /// </summary>
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
            DataContext = new CartViewModel();
        }
    }
}
