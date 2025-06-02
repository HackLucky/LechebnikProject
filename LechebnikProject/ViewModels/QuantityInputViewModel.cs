using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class QuantityInputViewModel : BaseViewModel
    {
        public Models.Medicine SelectedMedicine { get; set; }
        public string Quantity { get; set; }

        public class Medicine
        {
            public int MedicineId { get; set; }
            public string Name { get; set; }
            public int StockQuantity { get; set; }
            public decimal Price { get; set; }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        public QuantityInputViewModel(Models.Medicine medicine)
        {
            SelectedMedicine = medicine;
            Quantity = "1";

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

            decimal discount = AppContext.CurrentClient?.Discount ?? 0m;

            string query = @"INSERT INTO CartItems (UserId, MedicineId, Quantity, IsByPrescription, PrescriptionId, Discount)
                         VALUES (@UserId, @MedicineId, @Quantity, @IsByPrescription, @PrescriptionId, @Discount)";
            var parameters = new[]
            {
                new SqlParameter("@UserId", AppContext.CurrentUser.UserId),
                new SqlParameter("@MedicineId", SelectedMedicine.MedicineId),
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@IsByPrescription", false),
                new SqlParameter("@PrescriptionId", DBNull.Value),
                new SqlParameter("@Discount", discount)
            };
            DatabaseHelper.ExecuteNonQuery(query, parameters);

            MessageBox.Show("Препарат добавлен в корзину.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            WindowManager.ShowWindow<MedicineListWindow>();
        }
    }
}