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
            string query = "SELECT OrderDate AS [Дата заказа], TotalAmount AS [Сумма] FROM Orders ORDER BY OrderDate DESC";
            SalesData = DatabaseHelper.ExecuteQuery(query);
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