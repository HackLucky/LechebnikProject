using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class PrescriptionsViewModel : BaseViewModel
    {
        private DataTable _prescriptions;
        private object _selectedPrescription;

        public DataTable Prescriptions
        {
            get => _prescriptions;
            set => SetProperty(ref _prescriptions, value);
        }

        public object SelectedPrescription
        {
            get => _selectedPrescription;
            set => SetProperty(ref _selectedPrescription, value);
        }

        public ICommand AddPrescriptionCommand { get; }
        public ICommand EditPrescriptionCommand { get; }
        public ICommand GoBackCommand { get; }

        public PrescriptionsViewModel()
        {
            LoadPrescriptions();
            AddPrescriptionCommand = new RelayCommand(AddPrescription);
            EditPrescriptionCommand = new RelayCommand(EditPrescription, CanEditPrescription);
            GoBackCommand = new RelayCommand(GoBack);
        }

        private void LoadPrescriptions()
        {
            string query = "SELECT * FROM Prescriptions";
            Prescriptions = DatabaseHelper.ExecuteQuery(query);
        }

        private void AddPrescription(object parameter)
        {
            var prescriptionInputWindow = new PrescriptionInputWindow(new Medicine()); // Предполагаем выбор препарата в окне
            prescriptionInputWindow.ShowDialog();
            LoadPrescriptions();
        }

        private void EditPrescription(object parameter)
        {
            if (SelectedPrescription is DataRowView row)
            {
                var prescription = new Prescription
                {
                    PrescriptionId = Convert.ToInt32(row["PrescriptionId"]),
                    Series = row["Series"].ToString(),
                    MedicalInstitution = row["MedicalInstitution"].ToString(),
                    PatientLastName = row["PatientLastName"].ToString(),
                    PatientFirstName = row["PatientFirstName"].ToString(),
                    ICD10Code = row["ICD10Code"].ToString(),
                    Quantity = Convert.ToInt32(row["Quantity"]),
                    DiscountType = row["DiscountType"].ToString(),
                    MedicineId = Convert.ToInt32(row["MedicineId"]),
                    PharmacistId = Convert.ToInt32(row["PharmacistId"]),
                    ExpiryDate = Convert.ToDateTime(row["ExpiryDate"])
                };

                string query = "UPDATE Prescriptions SET Series = @Series, MedicalInstitution = @MedicalInstitution, PatientLastName = @PatientLastName, PatientFirstName = @PatientFirstName, ICD10Code = @ICD10Code, Quantity = @Quantity, DiscountType = @DiscountType, ExpiryDate = @ExpiryDate WHERE PrescriptionId = @PrescriptionId";
                SqlParameter[] parameters = {
                    new SqlParameter("@Series", prescription.Series),
                    new SqlParameter("@MedicalInstitution", prescription.MedicalInstitution),
                    new SqlParameter("@PatientLastName", prescription.PatientLastName),
                    new SqlParameter("@PatientFirstName", prescription.PatientFirstName),
                    new SqlParameter("@ICD10Code", prescription.ICD10Code),
                    new SqlParameter("@Quantity", prescription.Quantity),
                    new SqlParameter("@DiscountType", prescription.DiscountType),
                    new SqlParameter("@ExpiryDate", prescription.ExpiryDate),
                    new SqlParameter("@PrescriptionId", prescription.PrescriptionId)
                };

                DatabaseHelper.ExecuteNonQuery(query, parameters);
                LoadPrescriptions();
            }
        }

        private bool CanEditPrescription(object parameter) => SelectedPrescription != null;

        private void GoBack(object parameter)
        {
            var mainMenuWindow = new MainMenuWindow();
            mainMenuWindow.Show();
            (Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is PrescriptionsWindow))?.Close();
            Application.Current.MainWindow = mainMenuWindow;
        }
    }
}