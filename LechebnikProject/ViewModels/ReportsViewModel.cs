using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

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
            string query = @"
                SELECT o.OrderId, 
                       u.LastName + ' ' + u.FirstName + ' ' + ISNULL(u.MiddleName, '') AS UserFullName,
                       ISNULL(c.LastName + ' ' + c.FirstName + ' ' + ISNULL(c.MiddleName, ''), 'Не указан') AS ClientFullName,
                       (SELECT SUM(Quantity) FROM OrderDetails od WHERE od.OrderId = o.OrderId) AS TotalItems,
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
                Reports.Add(new OrderReport
                {
                    OrderId = Convert.ToInt32(row["OrderId"]),
                    UserFullName = row["UserFullName"].ToString(),
                    ClientFullName = row["ClientFullName"].ToString(),
                    TotalItems = Convert.ToInt32(row["TotalItems"]),
                    PaymentMethod = row["PaymentMethod"].ToString(),
                    TotalWithoutDiscount = Convert.ToDecimal(row["TotalWithoutDiscount"]),
                    DiscountPercentage = Convert.ToDecimal(row["DiscountPercentage"]),
                    TotalWithDiscount = Convert.ToDecimal(row["TotalWithDiscount"]),
                    OrderDateTime = row["OrderDateTime"].ToString()
                });
            }
        }

        public class OrderReport
        {
            public int OrderId { get; set; }
            public string UserFullName { get; set; }
            public string ClientFullName { get; set; }
            public int TotalItems { get; set; }
            public string PaymentMethod { get; set; }
            public decimal TotalWithoutDiscount { get; set; }
            public decimal DiscountPercentage { get; set; }
            public decimal TotalWithDiscount { get; set; }
            public string OrderDateTime { get; set; }
        }
    }
}