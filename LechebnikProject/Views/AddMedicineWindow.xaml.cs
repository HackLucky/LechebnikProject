using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class AddMedicineWindow : Window
    {
        public AddMedicineWindow()
        {
            InitializeComponent();
            DataContext = new AddMedicineViewModel();
        }
    }
}
