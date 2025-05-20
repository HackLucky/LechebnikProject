using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using System.Windows;

namespace LechebnikProject.Views
{
    /// <summary>
    /// Логика взаимодействия для MedicineDetailsWindow.xaml
    /// </summary>
    public partial class MedicineDetailsWindow : Window
    {
        public MedicineDetailsWindow(int medicineId)
        {
            InitializeComponent();
            DataContext = new MedicineDetailsViewModel(medicineId);
        }
    }
}
