using LechebnikProject.Helpers;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace LechebnikProject.ViewModels
{
    public class ClientLoginViewModel
    {
        public string Login { get; set; }
        public string Code { get; set; }

        public bool Authenticate()
        {
            string query = "SELECT * FROM Clients WHERE Login = @Login AND Code = @Code";
            var parameters = new[]
            {
                new SqlParameter("@Login", Login),
                new SqlParameter("@Code", Code)
            };
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
            if (dataTable.Rows.Count > 0)
            {
                // Можно сохранить данные клиента в AppContext, если нужно
                return true;
            }
            MessageBox.Show("Неверный логин или код.");
            return false;
        }
    }
}