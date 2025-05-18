using System.Windows;
using System.Windows.Input;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;

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
            var window = new MedicineListWindow();
            window.Show();
            (Application.Current.MainWindow as Window)?.Close();
            Application.Current.MainWindow = window;
        }
    }
}