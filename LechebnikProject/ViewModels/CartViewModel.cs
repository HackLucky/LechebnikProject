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
        private string _searchText;

        public decimal TotalAmount => CartItems?.Sum(item => item.Medicine.Price * item.Quantity * (1 - (AppContext.CurrentClient?.Discount ?? 0) / 100)) ?? 0;
        public bool CanCheckout => CartItems?.Any() == true;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadCartItems(_searchText);
            }
        }

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
            CheckoutCommand = new RelayCommand(ConfirmPayment);
            OnPropertyChanged(nameof(CanCheckout));
            CartItems.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalAmount));
            GoToMainMenuCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadCartItems(string searchText = "")
        {
            try
            {
                string query = @"
                    SELECT ci.*, m.Name, m.SerialNumber, m.Price, p.Series 
                    FROM CartItems ci 
                    JOIN Medicines m ON ci.MedicineId = m.MedicineId 
                    LEFT JOIN Prescriptions p ON ci.PrescriptionId = p.PrescriptionId 
                    WHERE ci.UserId = @UserId AND
                        (m.Name LIKE @SearchText OR
                        m.SerialNumber LIKE @SearchText OR
                        m.Price LIKE @SearchText OR
                        ci.Quantity LIKE @SearchText OR
                        p.Series LIKE @SearchText)";
                var parameters = new[] { 
                    new SqlParameter("@UserId", AppContext.CurrentUser.UserId),
                    new SqlParameter("@SearchText", $"%{searchText}%")
                };
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                CartItems = new ObservableCollection<CartItem>();
                foreach (DataRow row in dataTable.Rows)
                {
                    bool isByPrescription = Convert.ToBoolean(row["IsByPrescription"]);
                    decimal discount = 0m;
                    if (isByPrescription)
                    {
                        string discountType = row["DiscountType"]?.ToString();
                        discount = discountType == "50%" ? 50m : discountType == "Бесплатно" ? 100m : 0m;
                    }
                    else if (AppContext.CurrentClient != null)
                    {
                        discount = AppContext.CurrentClient.Discount;
                    }

                    CartItems.Add(new CartItem
                    {
                        CartItemId = Convert.ToInt32(row["CartItemId"]),
                        Medicine = new Medicine
                        {
                            MedicineId = Convert.ToInt32(row["MedicineId"]),
                            Name = row["Name"].ToString(),
                            SerialNumber = row["SerialNumber"].ToString(),
                            Price = Convert.ToDecimal(row["Price"])
                        },
                        Quantity = Convert.ToInt32(row["Quantity"]),
                        IsByPrescription = isByPrescription,
                        PrescriptionId = row["PrescriptionId"] != DBNull.Value ? Convert.ToInt32(row["PrescriptionId"]) : (int?)null,
                        Prescription = isByPrescription ? new Prescription { Series = row["Series"]?.ToString() } : null,
                        Discount = discount
                    });
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Remove(object parameter)
        {
            try
            {
                if (parameter is CartItem item)
                {
                    string query = "DELETE FROM CartItems WHERE CartItemId = @CartItemId";
                    var parameters = new[] { new SqlParameter("@CartItemId", item.CartItemId) };
                    DatabaseHelper.ExecuteNonQuery(query, parameters);
                    CartItems.Remove(item);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Clear(object parameter)
        {
            try
            {
                string query = "DELETE FROM CartItems WHERE UserId = @UserId";
                var parameters = new[] { new SqlParameter("@UserId", AppContext.CurrentUser.UserId) };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                CartItems.Clear();
                AppContext.CurrentClient = null;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void ConfirmPayment(object parameter)
        {
            if (CartItems.Count == 0) { MessageBox.Show("Корзина пуста.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); return; }
            if (MessageBox.Show("Оплата подтверждена?", "Подтверждение.", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                string orderQuery = @"INSERT INTO Orders (UserId, ClientId, OrderDate, PaymentMethod, TotalAmount, DiscountApplied, DiscountPercentage) 
                    OUTPUT INSERTED.OrderId 
                    VALUES (@UserId, @ClientId, @OrderDate, @PaymentMethod, @TotalAmount, @DiscountApplied, @DiscountPercentage)";
                var orderParams = new[]
                {
                    new SqlParameter("@UserId", AppContext.CurrentUser.UserId),
                    new SqlParameter("@ClientId", AppContext.CurrentClient?.ClientId ?? (object)DBNull.Value),
                    new SqlParameter("@OrderDate", DateTime.Now),
                    new SqlParameter("@PaymentMethod", "QR-код"),
                    new SqlParameter("@TotalAmount", TotalAmount),
                    new SqlParameter("@DiscountApplied", AppContext.CurrentClient != null && AppContext.CurrentClient.Discount > 0),
                    new SqlParameter("@DiscountPercentage", AppContext.CurrentClient?.Discount ?? (object)DBNull.Value)
                };
                int orderId = (int)DatabaseHelper.ExecuteScalar(orderQuery, orderParams);

                foreach (var item in CartItems)
                {
                    string updateStockQuery = "UPDATE Medicines SET StockQuantity = StockQuantity - @Quantity WHERE MedicineId = @MedicineId";
                    var stockParams = new[]
                    {
                        new SqlParameter("@Quantity", item.Quantity),
                        new SqlParameter("@MedicineId", item.Medicine.MedicineId)
                    };
                    DatabaseHelper.ExecuteNonQuery(updateStockQuery, stockParams);

                    string detailQuery = @"INSERT INTO OrderDetails (OrderId, MedicineId, Quantity, PricePerUnit, DiscountApplied, DiscountedPricePerUnit) 
                        VALUES (@OrderId, @MedicineId, @Quantity, @PricePerUnit, @DiscountApplied, @DiscountedPricePerUnit)";
                    var detailParams = new[]
                    {
                        new SqlParameter("@OrderId", orderId),
                        new SqlParameter("@MedicineId", item.Medicine.MedicineId),
                        new SqlParameter("@Quantity", item.Quantity),
                        new SqlParameter("@PricePerUnit", item.Medicine.Price),
                        new SqlParameter("@DiscountApplied", item.IsByPrescription || (AppContext.CurrentClient?.Discount > 0)),
                        new SqlParameter("@DiscountedPricePerUnit", item.IsByPrescription ? item.Medicine.Price * 0.5m : item.Medicine.Price * (1 - (AppContext.CurrentClient?.Discount ?? 0) / 100m))
                    };
                    DatabaseHelper.ExecuteNonQuery(detailQuery, detailParams);
                }

                string query = "DELETE FROM CartItems WHERE UserId = @UserId";
                var parameters = new[] { new SqlParameter("@UserId", AppContext.CurrentUser.UserId) };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                CartItems.Clear();
                AppContext.CurrentClient = null;
                MessageBox.Show("Оплата успешно подтверждена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                WindowManager.ShowWindow<MainMenuWindow>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}