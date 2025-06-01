using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class CartViewModel : BaseViewModel
    {
        private ObservableCollection<CartItem> _cartItems;
        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set => SetProperty(ref _cartItems, value);
        }
        public decimal TotalAmount => CartItems?.Sum(item => item.Medicine.Price * item.Quantity * (1 - (AppContext.CurrentClient?.Discount ?? 0) / 100)) ?? 0;
        public bool CanCheckout => CartItems?.Any() == true;

        public ICommand RemoveCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand GoToMainMenuCommand { get; }

        public CartViewModel()
        {
            if (AppContext.CurrentUser == null)
            {
                MessageBox.Show("Пользователь не авторизован.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                WindowManager.ShowWindow<LoginWindow>();
                return;
            }
            LoadCartItems();
            CartItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalAmount));
            RemoveCommand = new RelayCommand(Remove);
            ClearCommand = new RelayCommand(Clear);
            CheckoutCommand = new RelayCommand(Checkout);
            OnPropertyChanged(nameof(CanCheckout));
            CartItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalAmount));
            GoToMainMenuCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadCartItems()
        {
            string query = @"
                SELECT ci.*, m.Name, m.Price 
                FROM CartItems ci 
                JOIN Medicines m ON ci.MedicineId = m.MedicineId 
                WHERE ci.UserId = @UserId";
            var parameters = new[] { new SqlParameter("@UserId", AppContext.CurrentUser.UserId) };
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
            CartItems = new ObservableCollection<CartItem>();
            foreach (DataRow row in dataTable.Rows)
            {
                CartItems.Add(new CartItem
                {
                    CartItemId = Convert.ToInt32(row["CartItemId"]),
                    Medicine = new Medicine
                    {
                        MedicineId = Convert.ToInt32(row["MedicineId"]),
                        Name = row["Name"].ToString(),
                        Price = Convert.ToDecimal(row["Price"])
                    },
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    IsByPrescription = Convert.ToBoolean(row["IsByPrescription"]),
                    PrescriptionId = row["PrescriptionId"] != DBNull.Value ? Convert.ToInt32(row["PrescriptionId"]) : (int?)null
                });
            }
        }

        private void Remove(object parameter)
        {
            if (parameter is CartItem item)
            {
                string query = "DELETE FROM CartItems WHERE CartItemId = @CartItemId";
                var parameters = new[] { new SqlParameter("@CartItemId", item.CartItemId) };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                CartItems.Remove(item);
            }
        }

        private void Clear(object parameter)
        {
            string query = "DELETE FROM CartItems WHERE UserId = @UserId";
            var parameters = new[] { new SqlParameter("@UserId", AppContext.CurrentUser.UserId) };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            CartItems.Clear();
            AppContext.CurrentClient = null;
        }

        private void Checkout(object parameter)
        {
            try
            {
                var result = MessageBox.Show("Подтверждаете успешную оплату?", "Окно подтверждения оплаты.", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;

                const string orderSql = "INSERT INTO Orders (UserId, OrderDate, TotalAmount) OUTPUT INSERTED.OrderId VALUES (@UserId, GETDATE(), @Total)";
                var orderPrms = new[]
                {
                    new SqlParameter("@UserId", AppContext.CurrentUser.UserId),
                    new SqlParameter("@Total", TotalAmount)
                };
                int orderId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(orderSql, orderPrms));

                foreach (var item in CartItems)
                {
                    const string detailSql = "INSERT INTO OrderDetails (OrderId, MedicineId, Quantity, Price) VALUES (@OrderId, @MedId, @Qty, @Price)";
                    var detPrms = new[]
                    {
                        new SqlParameter("@OrderId", orderId),
                        new SqlParameter("@MedId", item.Medicine.MedicineId),
                        new SqlParameter("@Qty", item.Quantity),
                        new SqlParameter("@Price", item.Medicine.Price)
                    };
                    DatabaseHelper.ExecuteNonQuery(detailSql, detPrms);

                    const string stockSql = "UPDATE Medicines SET StockQuantity = StockQuantity - @Qty WHERE MedicineId = @MedId";
                    var stockPrms = new[]
                    {
                        new SqlParameter("@Qty", item.Quantity),
                        new SqlParameter("@MedId", item.Medicine.MedicineId)
                    };
                    DatabaseHelper.ExecuteNonQuery(stockSql, stockPrms);
                }

                MessageBox.Show("Оплата подтверждена.", "Информирование", MessageBoxButton.OK, MessageBoxImage.Information);
                WindowManager.ShowWindow<MedicineListWindow>();
            }
            catch (SqlException ex)
            {
                Logger.LogError("Ошибка при оформлении заказа.", ex);
                MessageBox.Show("Не удалось оформить заказ.", "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}