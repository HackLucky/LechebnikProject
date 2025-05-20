using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class ManageReportsWindow : Window
    {
        public ManageReportsWindow()
        {
            InitializeComponent();
            DataContext = new ManageReportsViewModel();
        }
    }
}