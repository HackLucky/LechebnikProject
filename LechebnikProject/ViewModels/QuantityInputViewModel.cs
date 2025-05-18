using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class QuantityInputViewModel
    {
        public Medicine SelectedMedicine { get; set; }
        public int Quantity { get; set; }
        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public QuantityInputViewModel(Medicine medicine)
        {
            SelectedMedicine = medicine;
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Add(object parameter)
        {
            if (Quantity <= 0 || Quantity > SelectedMedicine.StockQuantity)
            {
                MessageBox.Show("Некорректное количество или отсутствие товара на складе.");
                return;
            }
            var cartItem = AppContext.CartItems.Find(c => c.Medicine.MedicineId == SelectedMedicine.MedicineId);
            if (cartItem != null)
                cartItem.Quantity += Quantity;
            else
                AppContext.CartItems.Add(new CartItem { Medicine = SelectedMedicine, Quantity = Quantity });
            GoToMedicineList(parameter);
        }

        private void Cancel(object parameter)
        {
            GoToMedicineList(parameter);
        }

        private void GoToMedicineList(object parameter)
        {
            var window = new MedicineListWindow();
            window.Show();
            (Application.Current.MainWindow as Window)?.Close();
            Application.Current.MainWindow = window;
        }
    }
}