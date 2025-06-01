using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
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
