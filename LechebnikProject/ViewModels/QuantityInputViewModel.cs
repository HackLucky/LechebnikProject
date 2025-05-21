using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System.Linq;
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
        public ICommand GoToMedicineListCommand { get; }

        public QuantityInputViewModel(Medicine medicine)
        {
            SelectedMedicine = medicine;
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
            GoToMedicineListCommand = new RelayCommand(o => WindowManager.ShowWindow<MedicineListWindow>());
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
            GoToMedicineList();
        }

        private void Cancel(object parameter)
        {
            GoToMedicineList();
        }

        private void GoToMedicineList()
        {
            WindowManager.CloseWindow<QuantityInputWindow>();
            WindowManager.ShowWindow<MedicineListWindow>();
        }
    }
}