using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ReportsWindow : Window
    {
        public ReportsWindow()
        {
            InitializeComponent();
            SearchTextBox.Focus();
            DataContext = new ReportsViewModel();
        }
    }
}
