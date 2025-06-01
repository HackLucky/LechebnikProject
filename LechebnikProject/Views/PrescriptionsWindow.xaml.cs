using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class PrescriptionsWindow : Window
    {
        public PrescriptionsWindow()
        {
            InitializeComponent();
            DataContext = new PrescriptionsViewModel();
        }
    }
}
