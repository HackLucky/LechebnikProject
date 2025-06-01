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
        public string QuantityText { get; set; }
        public int Quantity { get; set; }

        private readonly Medicine _selectedMedicine;

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public QuantityInputViewModel(Medicine medicine)
        {
            _selectedMedicine = medicine ?? throw new ArgumentNullException(nameof(medicine));

            ConfirmCommand = new RelayCommand(Confirm);
            CancelCommand = new RelayCommand(_ => WindowManager.ShowWindow<MedicineListWindow>());
        }

        private void Confirm(object obj)
        {
            if (!int.TryParse(QuantityText, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Quantity = quantity;

            var cartItem = new CartItem
            {
                Medicine = _selectedMedicine,
                Quantity = quantity,
                IsByPrescription = false
            };

            AppContext.CartItems.Add(cartItem);

            decimal price = _selectedMedicine.Price;
            decimal discount = AppContext.CurrentClient?.Discount ?? 0;
            decimal total = price * quantity * (1 - discount / 100);

            MessageBox.Show(AppContext.CurrentClient != null
                ? $"Добавлено в корзину со скидкой {discount}%: {total} руб."
                : $"Добавлено в корзину без скидки: {price * quantity} руб.",
                "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

            WindowManager.ShowWindow<MedicineListWindow>();
        }
    }
}