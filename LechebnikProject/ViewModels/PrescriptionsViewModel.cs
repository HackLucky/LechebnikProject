using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Views;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class PrescriptionsViewModel : BaseViewModel
    {
        private DataTable _prescriptions;

        public DataTable Prescriptions
        {
            get => _prescriptions;
            set => SetProperty(ref _prescriptions, value);
        }
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                LoadPrescriptions();
            }
        }

        public ICommand GoBackCommand { get; }

        public PrescriptionsViewModel()
        {
            LoadPrescriptions();
            GoBackCommand = new RelayCommand(o => WindowManager.ShowWindow<MainMenuWindow>());
        }

        private void LoadPrescriptions()
        {
            try
            {
                string query = @"
                    SELECT 
                        p.PrescriptionId,
                        p.Series,
                        p.MedicalInstitution,
                        p.PatientLastName,
                        p.PatientFirstName,
                        p.PatientMiddleName,
                        p.ICD10Code,
                        p.Quantity,
                        p.DiscountType,
                        p.DoctorLastName,
                        p.DoctorFirstName,
                        p.DoctorMiddleName,
                        m.Name AS MedicineName,  -- Название препарата вместо ID
                        CONCAT(u.LastName, ' ', u.FirstName, ' ', u.MiddleName) AS PharmacistFullName,  -- ФИО фармацевта вместо ID
                        p.ExpiryDate
                    FROM Prescriptions p
                    LEFT JOIN Medicines m ON p.MedicineId = m.MedicineId
                    LEFT JOIN Users u ON p.PharmacistId = u.UserId
                    WHERE 
                        p.Series LIKE @SearchText OR
                        p.MedicalInstitution LIKE @SearchText OR
                        p.PatientLastName LIKE @SearchText OR
                        p.PatientFirstName LIKE @SearchText OR
                        p.PatientMiddleName LIKE @SearchText OR
                        p.ICD10Code LIKE @SearchText OR
                        CAST(p.Quantity AS NVARCHAR(20)) LIKE @SearchText OR
                        p.DiscountType LIKE @SearchText OR
                        p.DoctorLastName LIKE @SearchText OR
                        p.DoctorFirstName LIKE @SearchText OR
                        p.DoctorMiddleName LIKE @SearchText OR
                        m.Name LIKE @SearchText OR  -- Поиск по названию препарата
                        CONCAT(u.LastName, ' ', u.FirstName, ' ', u.MiddleName) LIKE @SearchText OR  -- Поиск по ФИО фармацевта
                        CONVERT(NVARCHAR, p.ExpiryDate, 104) LIKE @SearchText";
                string searchValue = string.IsNullOrEmpty(SearchText) ? "%" : $"%{SearchText}%";
                var parameters = new[] { new SqlParameter("@SearchText", searchValue) };
                Prescriptions = DatabaseHelper.ExecuteQuery(query, parameters);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Исключение.", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }
}