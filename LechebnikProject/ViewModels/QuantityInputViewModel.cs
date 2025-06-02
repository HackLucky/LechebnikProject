using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class QuantityInputViewModel : BaseViewModel
    {
        public Medicine SelectedMedicine { get; set; }
        private string _quantity;
        public string Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public QuantityInputViewModel(Medicine medicine)
        {
            if (medicine == null)
            {
                MessageBox.Show("Препарат не выбран.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                WindowManager.ShowWindow<MedicineListWindow>();
                return;
            }
            SelectedMedicine = medicine;

            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(_ => WindowManager.ShowWindow<MedicineListWindow>());
        }

        private void Confirm(object obj)
        {
            if (!int.TryParse(Quantity, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (quantity > SelectedMedicine.StockQuantity)
            {
                MessageBox.Show("Недостаточно товара на складе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AppContext.CartItems.Add(new CartItem
            {
                Medicine = SelectedMedicine,
                Quantity = quantity,
                IsByPrescription = false
            });

            MessageBox.Show("Препарат добавлен в корзину.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            WindowManager.ShowWindow<MedicineListWindow>();
        }
    }
}