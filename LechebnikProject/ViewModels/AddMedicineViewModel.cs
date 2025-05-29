using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class AddMedicineViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string SelectedForm { get; set; }
        public string WeightVolume { get; set; }
        public string SerialNumber { get; set; }
        public string Usage { get; set; }
        public string ActiveIngredient { get; set; }
        public string SelectedApplicationMethod { get; set; }
        public string SelectedAggregateState { get; set; }
        public string SelectedType { get; set; }
        public Manufacturer SelectedManufacturer { get; set; }
        public Supplier SelectedSupplier { get; set; }
        public string StockQuantity { get; set; }
        public bool RequiresPrescription { get; set; }
        public string Price { get; set; }

        public List<string> ApplicationMethods { get; } = new List<string> { "Ингаляционный", "Носовой", "Офтальмологический", "Парентеральный", "Пероральный", "Подъязычный", "Ректальный", "Топический", "Ушной" };
        public List<string> AggregateStates { get; } = new List<string> { "Газообразное", "Жидкое", "Мягкое", "Твёрдое" };
        public List<string> Types { get; } = new List<string> { "Патогенетическое", "Профилактическое", "Симптоматическое", "Стимулирующее", "Этиотропное" };
        public List<string> Forms { get; } = new List<string> { "Таблетки", "Сироп", "Мазь", "Капли", "Инъекции", "Капсулы", "Гель" };
        public List<Manufacturer> Manufacturers { get; set; }
        public List<Supplier> Suppliers { get; set; }

        public ICommand AddCommand { get; }
        public ICommand GoToMainMenuCommand { get; }

        public AddMedicineViewModel()
        {
            LoadManufacturers();
            LoadSuppliers();
            AddCommand = new RelayCommand(Add);
            GoToMainMenuCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadManufacturers()
        {
            string query = "SELECT * FROM Manufacturers";
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query);
            Manufacturers = dataTable.AsEnumerable().Select(row => new Manufacturer
            {
                ManufacturerId = row.Field<int>("ManufacturerId"),
                Name = row.Field<string>("Name"),
                Country = row.Field<string>("Country")
            }).ToList();
        }

        private void LoadSuppliers()
        {
            string query = "SELECT * FROM Suppliers";
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query);
            Suppliers = dataTable.AsEnumerable().Select(row => new Supplier
            {
                SupplierId = row.Field<int>("SupplierId"),
                Name = row.Field<string>("Name"),
                Country = row.Field<string>("Country")
            }).ToList();
        }

        private void Add(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Name) || SelectedForm == null || SelectedManufacturer == null || SelectedSupplier == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            if (!int.TryParse(StockQuantity, out int stock) || stock < 0)
            {
                MessageBox.Show("Некорректное количество на складе.");
                return;
            }

            if (!decimal.TryParse(Price, out decimal price) || price < 0)
            {
                MessageBox.Show("Некорректная цена.");
                return;
            }

            string query = "INSERT INTO Medicines (Name, Form, WeightVolume, SerialNumber, Usage, ActiveIngredient, ApplicationMethod, AggregateState, Type, ManufacturerId, SupplierId, StockQuantity, RequiresPrescription, Price) VALUES (@Name, @Form, @WeightVolume, @SerialNumber, @Usage, @ActiveIngredient, @ApplicationMethod, @AggregateState, @Type, @ManufacturerId, @SupplierId, @StockQuantity, @RequiresPrescription, @Price)";
            SqlParameter[] parameters = {
                new SqlParameter("@Name", Name),
                new SqlParameter("@Form", SelectedForm),
                new SqlParameter("@WeightVolume", WeightVolume ?? (object)DBNull.Value),
                new SqlParameter("@SerialNumber", SerialNumber ?? (object)DBNull.Value),
                new SqlParameter("@Usage", Usage ?? (object)DBNull.Value),
                new SqlParameter("@ActiveIngredient", ActiveIngredient ?? (object)DBNull.Value),
                new SqlParameter("@ApplicationMethod", SelectedApplicationMethod ?? (object)DBNull.Value),
                new SqlParameter("@AggregateState", SelectedAggregateState ?? (object)DBNull.Value),
                new SqlParameter("@Type", SelectedType ?? (object)DBNull.Value),
                new SqlParameter("@ManufacturerId", SelectedManufacturer.ManufacturerId),
                new SqlParameter("@SupplierId", SelectedSupplier.SupplierId),
                new SqlParameter("@StockQuantity", stock),
                new SqlParameter("@RequiresPrescription", RequiresPrescription),
                new SqlParameter("@Price", price)
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                MessageBox.Show("Препарат добавлен успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при добавлении препарата. {ex}");
            }
        }
    }
}