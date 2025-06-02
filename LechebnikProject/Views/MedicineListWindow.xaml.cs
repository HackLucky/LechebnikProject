using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class MedicineListWindow : Window
    {
        public MedicineListWindow()
        {
            InitializeComponent();
            SearchTextBox.Focus();
            DataContext = new MedicineListViewModel();
        }
    }
}
