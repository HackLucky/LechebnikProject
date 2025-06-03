using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class MedicineDetailsViewModel
    {
        public Medicine SelectedMedicine { get; set; }

        public ICommand GoBackCommand { get; }

        public MedicineDetailsViewModel(int medicineId)
        {
            try
            {
                GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MedicineListWindow>());

                string query = @"
                SELECT m.*, man.Name AS ManufacturerName, man.Country AS ManufacturerCountry, 
                       sup.Name AS SupplierName, sup.Country AS SupplierCountry 
                FROM Medicines m 
                JOIN Manufacturers man ON m.ManufacturerId = man.ManufacturerId 
                JOIN Suppliers sup ON m.SupplierId = sup.SupplierId 
                WHERE m.MedicineId = @MedicineId";
                var parameters = new[] { new SqlParameter("@MedicineId", medicineId) };
                DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    SelectedMedicine = new Medicine
                    {
                        MedicineId = Convert.ToInt32(row["MedicineId"]),
                        Name = row["Name"].ToString(),
                        Form = row["Form"].ToString(),
                        WeightVolume = row["WeightVolume"].ToString(),
                        SerialNumber = row["SerialNumber"].ToString(),
                        Usage = row["Usage"].ToString(),
                        ActiveIngredient = row["ActiveIngredient"].ToString(),
                        ApplicationMethod = row["ApplicationMethod"].ToString(),
                        AggregateState = row["AggregateState"].ToString(),
                        Type = row["Type"].ToString(),
                        ManufacturerId = Convert.ToInt32(row["ManufacturerId"]),
                        ManufacturerName = row["ManufacturerName"].ToString(),
                        SupplierId = Convert.ToInt32(row["SupplierId"]),
                        SupplierName = row["SupplierName"].ToString(),
                        ManufacturerCountry = row["ManufacturerCountry"].ToString(),
                        SupplierCountry = row["SupplierCountry"].ToString(),
                        StockQuantity = Convert.ToInt32(row["StockQuantity"]),
                        RequiresPrescription = Convert.ToBoolean(row["RequiresPrescription"]),
                        Price = Convert.ToDecimal(row["Price"])
                    };
                }
                GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MedicineListWindow>());
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}