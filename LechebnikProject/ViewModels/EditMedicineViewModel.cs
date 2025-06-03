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
    public class EditMedicineViewModel : BaseViewModel
    {
        private readonly int _medicineId;

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
        public List<Manufacturer> Manufacturers { get; private set; }
        public List<Supplier> Suppliers { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand GoToMainMenuCommand { get; }

        public EditMedicineViewModel()
        {
            LoadManufacturers();
            LoadSuppliers();
            SaveCommand = new RelayCommand(Save);
            GoToMainMenuCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadManufacturers()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Manufacturers");
                Manufacturers = dt.AsEnumerable()
                    .Select(r => new Manufacturer
                    {
                        ManufacturerId = r.Field<int>("ManufacturerId"),
                        Name = r.Field<string>("Name"),
                        Country = r.Field<string>("Country")
                    }).ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void LoadSuppliers()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Suppliers");
                Suppliers = dt.AsEnumerable()
                    .Select(r => new Supplier
                    {
                        SupplierId = r.Field<int>("SupplierId"),
                        Name = r.Field<string>("Name"),
                        Country = r.Field<string>("Country")
                    }).ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        public EditMedicineViewModel(Medicine medicine)
        {
            _medicineId = medicine.MedicineId;

            Name = medicine.Name;
            SelectedForm = medicine.Form;
            WeightVolume = medicine.WeightVolume;
            SerialNumber = medicine.SerialNumber;
            Usage = medicine.Usage;
            ActiveIngredient = medicine.ActiveIngredient;
            SelectedApplicationMethod = medicine.ApplicationMethod;
            SelectedAggregateState = medicine.AggregateState;
            SelectedType = medicine.Type;
            RequiresPrescription = medicine.RequiresPrescription;
            StockQuantity = medicine.StockQuantity.ToString();
            Price = medicine.Price.ToString("F2");

            LoadManufacturers();
            LoadSuppliers();

            SaveCommand = new RelayCommand(Save);
            GoToMainMenuCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void Save(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Name) || SelectedForm == null || SelectedManufacturer == null || SelectedSupplier == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(StockQuantity, out int stock) || stock < 0)
            {
                MessageBox.Show("Некорректное количество на складе.", "предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(Price, out decimal price) || price < 0)
            {
                MessageBox.Show("Некорректная цена.", "предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            const string sql = @"
                UPDATE Medicines SET 
                    Name=@Name, Form=@Form, WeightVolume=@WeightVolume,
                    SerialNumber=@SerialNumber, Usage=@Usage, ActiveIngredient=@ActiveIngredient,
                    ApplicationMethod=@AppMethod, AggregateState=@AggState, Type=@Type,
                    ManufacturerId=@ManId, SupplierId=@SupId, StockQuantity=@Stock, 
                    RequiresPrescription=@Req, Price=@Price
                WHERE MedicineId=@MedId";
            var prms = new[]
            {
                new SqlParameter("@Name", Name),
                new SqlParameter("@Form", SelectedForm),
                new SqlParameter("@WeightVolume", WeightVolume ?? (object)DBNull.Value),
                new SqlParameter("@SerialNumber", SerialNumber ?? (object)DBNull.Value),
                new SqlParameter("@Usage", Usage ?? (object)DBNull.Value),
                new SqlParameter("@ActiveIngredient", ActiveIngredient ?? (object)DBNull.Value),
                new SqlParameter("@AppMethod", SelectedApplicationMethod ?? (object)DBNull.Value),
                new SqlParameter("@AggState", SelectedAggregateState ?? (object)DBNull.Value),
                new SqlParameter("@Type", SelectedType ?? (object)DBNull.Value),
                new SqlParameter("@ManId", SelectedManufacturer.ManufacturerId),
                new SqlParameter("@SupId", SelectedSupplier.SupplierId),
                new SqlParameter("@Stock", stock),
                new SqlParameter("@Req", RequiresPrescription),
                new SqlParameter("@Price", price),
                new SqlParameter("@MedId", _medicineId)
            };

            try
            {
                DatabaseHelper.ExecuteNonQuery(sql, prms);
                MessageBox.Show("Данные препарата обновлены.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
                WindowManager.ShowWindow<ManageMedicinesWindow>();
            }
            catch (SqlException ex)
            {
                Logger.LogError("Ошибка при обновлении препарата", ex);
                MessageBox.Show("Не удалось сохранить изменения. Проверьте данные или обратитесь к администратору: andrej_sok@mail.ru.", "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}