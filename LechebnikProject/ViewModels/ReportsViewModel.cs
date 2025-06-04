using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using static LechebnikProject.ViewModels.ManageReportsViewModel;

namespace LechebnikProject.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        public ObservableCollection<OrderReport> Reports { get; set; }

        public ICommand GoBackCommand { get; }

        public ReportsViewModel()
        {
            LoadReports();
            GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadReports()
        {
            try
            {
                string query = @"
                    SELECT 
                        o.OrderId, 
                        u.LastName + ' ' + u.FirstName + ' ' + ISNULL(u.MiddleName, '') AS UserFullName,
                        ISNULL(c.LastName + ' ' + c.FirstName + ' ' + ISNULL(c.MiddleName, ''), 'Не указан') AS ClientFullName,
                        ISNULL((SELECT SUM(od.Quantity) FROM OrderDetails od WHERE od.OrderId = o.OrderId), 0) AS TotalItems,
                        o.PaymentMethod,
                        o.TotalAmount AS TotalWithoutDiscount,
                        ISNULL(o.DiscountPercentage, 0) AS DiscountPercentage,
                        o.TotalAmount * (1 - ISNULL(o.DiscountPercentage, 0) / 100) AS TotalWithDiscount,
                        CONVERT(varchar, o.OrderDate, 104) + ' ' + CONVERT(varchar, o.OrderDate, 108) AS OrderDateTime
                    FROM Orders o
                    JOIN Users u ON o.UserId = u.UserId
                    LEFT JOIN Clients c ON o.ClientId = c.ClientId";
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query);
                Reports = new ObservableCollection<OrderReport>();
                foreach (DataRow row in dataTable.Rows)
                {
                    // Безопасное приведение DBNull -> 0 или ""
                    var totalItems = row["TotalItems"] != DBNull.Value
                        ? Convert.ToInt32(row["TotalItems"])
                        : 0;
                    var discountPercentage = row["DiscountPercentage"] != DBNull.Value
                        ? Convert.ToDecimal(row["DiscountPercentage"])
                        : 0m;
                    var totalWithoutDiscount = row["TotalWithoutDiscount"] != DBNull.Value
                        ? Convert.ToDecimal(row["TotalWithoutDiscount"])
                        : 0m;
                    var totalWithDiscount = row["TotalWithDiscount"] != DBNull.Value
                        ? Convert.ToDecimal(row["TotalWithDiscount"])
                        : totalWithoutDiscount * (1 - discountPercentage / 100m);

                    Reports.Add(new OrderReport
                    {
                        OrderId = Convert.ToInt32(row["OrderId"]),
                        UserFullName = row["UserFullName"].ToString(),
                        ClientFullName = row["ClientFullName"].ToString(),
                        TotalItems = totalItems,
                        PaymentMethod = row["PaymentMethod"].ToString(),
                        TotalWithoutDiscount = totalWithoutDiscount,
                        DiscountPercentage = discountPercentage,
                        TotalWithDiscount = totalWithDiscount,
                        OrderDateTime = row["OrderDateTime"].ToString()
                    });
                }
                OnPropertyChanged(nameof(Reports));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}