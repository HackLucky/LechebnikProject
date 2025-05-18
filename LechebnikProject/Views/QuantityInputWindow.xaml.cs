using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для QuantityInputWindow.xaml
    /// </summary>
    public partial class QuantityInputWindow : Window
    {
        public QuantityInputWindow(Medicine medicine)
        {
            InitializeComponent();
            DataContext = new QuantityInputViewModel(medicine);
        }
    }
}
