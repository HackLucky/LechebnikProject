using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        public DataTable SalesData { get; set; }
        public ICommand GoBackCommand { get; }

        public ReportsViewModel()
        {
            LoadSalesData();
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void LoadSalesData()
        {
            string query = @"
                SELECT o.OrderId AS [Номер], o.OrderDate AS [Дата и время], 
                       o.TotalAmount AS [Сумма], u.LastName AS [Фамилия], 
                       u.FirstName AS [Имя], u.MiddleName AS [Отчество] 
                FROM Orders o 
                JOIN Users u ON o.UserId = u.UserId";
            SalesData = DatabaseHelper.ExecuteQuery(query);
            foreach (DataColumn column in SalesData.Columns)
            {
                if (column.ColumnName == "Отчество" && SalesData.AsEnumerable().All(row => row.IsNull(column)))
                    column.DefaultValue = "Не указано";
            }
        }

        private void GoBack(object parameter)
        {
            var mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is ReportsWindow))?.Close();
            Application.Current.MainWindow = mainMenuWindow;
        }
    }
}