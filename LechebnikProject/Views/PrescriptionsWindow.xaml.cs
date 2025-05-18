using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для PrescriptionsWindow.xaml
    /// </summary>
    public partial class PrescriptionsWindow : Window
    {
        public PrescriptionsWindow()
        {
            InitializeComponent();
            DataContext = new PrescriptionsViewModel();
        }
    }
}
