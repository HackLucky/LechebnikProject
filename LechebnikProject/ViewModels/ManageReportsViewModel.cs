using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class ManageReportsViewModel : BaseViewModel
    {
        private DataTable _reports;
        private DataRowView _selectedReport;

        public DataTable Reports
        {
            get => _reports;
            set => SetProperty(ref _reports, value);
        }

        public DataRowView SelectedReport
        {
            get => _selectedReport;
            set => SetProperty(ref _selectedReport, value);
        }

        public ICommand DeleteReportCommand { get; }
        public ICommand GoBackCommand { get; }

        public ManageReportsViewModel()
        {
            LoadReports();
            DeleteReportCommand = new RelayCommand(DeleteReport, CanDeleteReport);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void LoadReports()
        {
            string query = @"
                SELECT o.OrderId AS [Номер], o.OrderDate AS [Дата и время], 
                       o.TotalAmount AS [Сумма], u.LastName AS [Фамилия], 
                       u.FirstName AS [Имя], u.MiddleName AS [Отчество] 
                FROM Orders o 
                JOIN Users u ON o.UserId = u.UserId";
            Reports = DatabaseHelper.ExecuteQuery(query);
        }

        private void DeleteReport(object parameter)
        {
            if (SelectedReport != null)
            {
                int orderId = (int)SelectedReport["Номер"];
                string query = "DELETE FROM Orders WHERE OrderId = @OrderId";
                var parameters = new[] { new SqlParameter("@OrderId", orderId) };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadReports(); // Обновляем таблицу после удаления
                MessageBox.Show("Отчёт успешно удалён!");
            }
        }

        private bool CanDeleteReport(object parameter) => SelectedReport != null;

        private void GoBack(object parameter)
        {
            var adminPanelWindow = new AdminPanelWindow();
            adminPanelWindow.Show();
            Application.Current.Windows.OfType<ManageReportsWindow>().FirstOrDefault()?.Close();
        }
    }
}