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
        public QuantityInputWindow()
        {
            InitializeComponent();
        }

        public QuantityInputWindow(Medicine medicine) : this()
        {
            DataContext = new QuantityInputViewModel(medicine);
        }
    }
}
