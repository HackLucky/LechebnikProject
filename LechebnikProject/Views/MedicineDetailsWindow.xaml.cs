using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    public partial class MedicineDetailsWindow : Window
    {
        public MedicineDetailsWindow()
        {
            InitializeComponent();
            MedicineIdTextBox.Focus();
        }

        public MedicineDetailsWindow(int medicineId) : this()
        {
            DataContext = new MedicineDetailsViewModel(medicineId);
        }
    }
}
