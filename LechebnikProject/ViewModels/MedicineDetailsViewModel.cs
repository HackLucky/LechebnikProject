using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class MedicineDetailsViewModel
    {
        public Medicine SelectedMedicine { get; set; }
        public ICommand GoBackCommand { get; }

        public MedicineDetailsViewModel(Medicine medicine)
        {
            SelectedMedicine = medicine;
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void GoBack(object parameter)
        {
            var medicineListWindow = new MedicineListWindow();
            medicineListWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MedicineDetailsWindow))?.Close();
            Application.Current.MainWindow = medicineListWindow;
        }
    }
}