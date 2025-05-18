using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class MedicineListViewModel
    {
        public List<Medicine> Medicines { get; set; }
        public string SearchText { get; set; }
        public ICommand SearchCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand AddToCartByPrescriptionCommand { get; }
        public ICommand GoToCartCommand { get; }
        public ICommand GoBackCommand { get; }

        public MedicineListViewModel()
        {
            LoadMedicines();
            SearchCommand = new RelayCommand(Search);
            ViewDetailsCommand = new RelayCommand(ViewDetails);
            AddToCartCommand = new RelayCommand(AddToCart, CanAddToCart);
            AddToCartByPrescriptionCommand = new RelayCommand(AddToCartByPrescription);
            GoToCartCommand = new RelayCommand(GoToCart);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void LoadMedicines(string searchText = "")
        {
            string query = "SELECT * FROM Medicines WHERE Name LIKE @SearchText OR SerialNumber LIKE @SearchText";
            var parameters = new[] { new SqlParameter("@SearchText", $"%{searchText}%") };
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
            Medicines = new List<Medicine>();
            foreach (DataRow row in dataTable.Rows)
            {
                Medicines.Add(new Medicine
                {
                    MedicineId = Convert.ToInt32(row["MedicineId"]),
                    Name = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    StockQuantity = Convert.ToInt32(row["StockQuantity"]),
                    RequiresPrescription = Convert.ToBoolean(row["RequiresPrescription"]),
                    ManufacturerId = Convert.ToInt32(row["ManufacturerId"])
                });
            }
        }

        private void Search(object parameter)
        {
            LoadMedicines(SearchText ?? "");
        }

        private void ViewDetails(object parameter)
        {
            if (parameter is Medicine medicine)
            {
                var window = new MedicineDetailsWindow(medicine);
                window.Show();
                (Application.Current.MainWindow as Window)?.Close();
                Application.Current.MainWindow = window;
            }
        }

        private void AddToCart(object parameter)
        {
            if (parameter is Medicine medicine)
            {
                if (medicine.RequiresPrescription)
                {
                    MessageBox.Show("Требуется рецепт. Используйте 'По рецепту'.");
                    return;
                }
                var window = new QuantityInputWindow(medicine);
                window.Show();
                (Application.Current.MainWindow as Window)?.Close();
                Application.Current.MainWindow = window;
            }
        }

        private bool CanAddToCart(object parameter)
        {
            return parameter is Medicine medicine && !medicine.RequiresPrescription;
        }

        private void AddToCartByPrescription(object parameter)
        {
            if (parameter is Medicine medicine)
            {
                var window = new PrescriptionInputWindow(medicine);
                window.Show();
                (Application.Current.MainWindow as Window)?.Close();
                Application.Current.MainWindow = window;
            }
        }

        private void GoToCart(object parameter)
        {
            var window = new CartWindow();
            window.Show();
            (Application.Current.MainWindow as Window)?.Close();
            Application.Current.MainWindow = window;
        }

        private void GoBack(object parameter)
        {
            var window = new MainMenuWindow();
            window.Show();
            (Application.Current.MainWindow as Window)?.Close();
            Application.Current.MainWindow = window;
        }
    }
}