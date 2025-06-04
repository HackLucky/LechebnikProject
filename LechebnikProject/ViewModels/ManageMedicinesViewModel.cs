using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.ViewModels;
using LechebnikProject.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

public class ManageMedicinesViewModel : BaseViewModel
{
    public ObservableCollection<Medicine> Medicines { get; set; }
    public Medicine SelectedMedicine { get; set; }
    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            LoadMedicines(_searchText);
        }
    }

    public ICommand AddCommand => new RelayCommand(o => WindowManager.ShowWindow<AddMedicineWindow>());
    public ICommand DeleteCommand => new RelayCommand(Delete, o => SelectedMedicine != null);
    public ICommand GoBackCommand => new RelayCommand(o => WindowManager.ShowWindow<AdminPanelWindow>());
    public ICommand EditCommand => new RelayCommand(o => { if (SelectedMedicine == null) return;
        WindowManager.ShowWindow<EditMedicineWindow>(w => w.DataContext = new EditMedicineViewModel(SelectedMedicine));
    });

    public ManageMedicinesViewModel()
    {
        LoadMedicines();
    }

    private void LoadMedicines(string searchText = "")
    {
        try
        {
            string query = @"
                SELECT m.*, man.Name AS ManufacturerName, sup.Name AS SupplierName
                    FROM Medicines m
                    JOIN Manufacturers man ON m.ManufacturerId = man.ManufacturerId
                    JOIN Suppliers sup ON m.SupplierId = sup.SupplierId
                WHERE m.Name LIKE @SearchText 
                       OR m.SerialNumber LIKE @SearchText 
                       OR m.Price LIKE @SearchText  
                       OR m.StockQuantity LIKE @SearchText 
                       OR man.Name LIKE @SearchText
                       OR sup.Name LIKE @SearchText";
            var parameters = new[] { new SqlParameter("@SearchText", $"%{searchText}%") };
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
            var medicinesList = new ObservableCollection<Medicine>();
            foreach (DataRow row in dataTable.Rows)
            {
                medicinesList.Add(new Medicine
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
                    SupplierName = row["Name"].ToString() == row["SupplierName"].ToString() ? row["SupplierName"].ToString() : row["SupplierName"].ToString(),
                    StockQuantity = Convert.ToInt32(row["StockQuantity"]),
                    RequiresPrescription = Convert.ToBoolean(row["RequiresPrescription"]),
                    Price = Convert.ToDecimal(row["Price"])
                });
            }
            Medicines = medicinesList;
            OnPropertyChanged(nameof(Medicines));
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void Delete(object parameter)
    {
        try
        {
            if (SelectedMedicine == null) return;

            string query = "DELETE FROM Medicines WHERE MedicineId = @MedicineId";
            var parameters = new[] { new SqlParameter("@MedicineId", SelectedMedicine.MedicineId) };
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            Medicines.Remove(SelectedMedicine);
            MessageBox.Show("Препарат удалён.", "Информирование.", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}