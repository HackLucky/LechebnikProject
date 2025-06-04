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
    public ICommand DeleteCommand => new RelayCommand(Delete);
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
                SELECT m.*, man.Name AS ManufacturerName 
                FROM Medicines m 
                JOIN Manufacturers man ON m.ManufacturerId = man.ManufacturerId 
                WHERE m.Name LIKE @SearchText OR m.SerialNumber LIKE @SearchText OR
                    m.Price LIKE @SearchText  OR m.StockQuantity LIKE @SearchText OR
                    man.Name LIKE @SearchText";
            var parameters = new[] { new SqlParameter("@SearchText", $"%{searchText}%") };
            DataTable dataTable = DatabaseHelper.ExecuteQuery(query, parameters);
            var medicinesList = new List<Medicine>();
            foreach (DataRow row in dataTable.Rows)
            {
                medicinesList.Add(new Medicine
                {
                    MedicineId = Convert.ToInt32(row["MedicineId"]),
                    Name = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    StockQuantity = Convert.ToInt32(row["StockQuantity"]),
                    RequiresPrescription = Convert.ToBoolean(row["RequiresPrescription"]),
                    ManufacturerId = Convert.ToInt32(row["ManufacturerId"]),
                    ManufacturerName = row["ManufacturerName"].ToString()
                });
            }
            Medicines = new ObservableCollection<Medicine>(medicinesList);
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void Delete(object parameter)
    {
        try
        {
            if (SelectedMedicine != null)
            {
                string query = "DELETE FROM Medicines WHERE MedicineId = @MedicineId";
                var parameters = new[] { new SqlParameter("@MedicineId", SelectedMedicine.MedicineId) };
                DatabaseHelper.ExecuteNonQuery(query, parameters);
                Medicines.Remove(SelectedMedicine);
            }
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
    }
}