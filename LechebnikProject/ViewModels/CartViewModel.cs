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
        public ICommand GoBackCommand { get; }

        public CartViewModel()
        {
            LoadCartItems();
            CartItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalAmount));
            RemoveCommand = new RelayCommand(Remove);
            ClearCommand = new RelayCommand(Clear);
            CheckoutCommand = new RelayCommand(Checkout);
            GoBackCommand = new RelayCommand(GoBack);
            OnPropertyChanged(nameof(CanCheckout)); // Обновляем при инициализации
            CartItems.CollectionChanged += (s, e) => { OnPropertyChanged(nameof(TotalAmount)); OnPropertyChanged(nameof(CanCheckout)); };
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
                    Medicine = new Medicine { MedicineId = Convert.ToInt32(row["MedicineId"]), Name = row["Name"].ToString(), Price = Convert.ToDecimal(row["Price"]) },
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    IsByPrescription = Convert.ToBoolean(row["IsByPrescription"]),
                    PrescriptionId = (int)(row["PrescriptionId"] != DBNull.Value ? Convert.ToInt32(row["PrescriptionId"]) : (int?)null)
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
            AppContext.CurrentClient = null; // Сбрасываем клиента после очистки
        }

        private void Checkout(object parameter)
        {
            decimal total = TotalAmount;
            string checkBalanceQuery = "SELECT Balance FROM Accounts WHERE UserId = @UserId";
            var balanceParam = new[] { new SqlParameter("@UserId", AppContext.CurrentUser.UserId) };
            object balanceObj = DatabaseHelper.ExecuteScalar(checkBalanceQuery, balanceParam);
            decimal balance = balanceObj != null ? Convert.ToDecimal(balanceObj) : 0;

            if (balance < total)
            {
                MessageBox.Show("Недостаточно средств на счёте!");
                return;
            }

            string updateBalanceQuery = "UPDATE Accounts SET Balance = Balance - @Amount WHERE UserId = @UserId";
            var updateParams = new[]
            {
                new SqlParameter("@Amount", total),
                new SqlParameter("@UserId", AppContext.CurrentUser.UserId)
            };
            DatabaseHelper.ExecuteNonQuery(updateBalanceQuery, updateParams);

            string insertOrderQuery = "INSERT INTO Orders (UserId, ClientId, OrderDate, PaymentMethod, TotalAmount, DiscountApplied, DiscountPercentage) VALUES (@UserId, @ClientId, @OrderDate, @PaymentMethod, @TotalAmount, @DiscountApplied, @DiscountPercentage); SELECT SCOPE_IDENTITY();";
            var orderParams = new[]
            {
                new SqlParameter("@UserId", AppContext.CurrentUser.UserId),
                new SqlParameter("@ClientId", AppContext.CurrentClient?.ClientId ?? (object)DBNull.Value),
                new SqlParameter("@OrderDate", DateTime.Now),
                new SqlParameter("@PaymentMethod", "Виртуальный счёт"),
                new SqlParameter("@TotalAmount", total),
                new SqlParameter("@DiscountApplied", AppContext.CurrentClient != null),
                new SqlParameter("@DiscountPercentage", AppContext.CurrentClient?.Discount ?? (object)DBNull.Value)
            };
            int orderId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertOrderQuery, orderParams));

            foreach (var item in CartItems)
            {
                string insertDetailQuery = "INSERT INTO OrderDetails (OrderId, MedicineId, Quantity, PricePerUnit, DiscountApplied, DiscountedPricePerUnit) VALUES (@OrderId, @MedicineId, @Quantity, @PricePerUnit, @DiscountApplied, @DiscountedPricePerUnit)";
                var detailParams = new[]
                {
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@MedicineId", item.Medicine.MedicineId),
                    new SqlParameter("@Quantity", item.Quantity),
                    new SqlParameter("@PricePerUnit", item.Medicine.Price),
                    new SqlParameter("@DiscountApplied", AppContext.CurrentClient != null),
                    new SqlParameter("@DiscountedPricePerUnit", AppContext.CurrentClient != null ? item.Medicine.Price * (1 - AppContext.CurrentClient.Discount / 100) : (object)DBNull.Value)
                };
                DatabaseHelper.ExecuteNonQuery(insertDetailQuery, detailParams);

                string updateStockQuery = "UPDATE Medicines SET StockQuantity = StockQuantity - @Quantity WHERE MedicineId = @MedicineId";
                var stockParams = new[]
                {
                    new SqlParameter("@Quantity", item.Quantity),
                    new SqlParameter("@MedicineId", item.Medicine.MedicineId)
                };
                DatabaseHelper.ExecuteNonQuery(updateStockQuery, stockParams);
            }

            Clear(null);
            MessageBox.Show("Оплата успешно выполнена!");
            GoBack(null);
        }

        private void GoBack(object parameter)
        {
            var mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is CartWindow))?.Close();
        }
    }
}