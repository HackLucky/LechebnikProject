using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class PrescriptionInputViewModel : BaseViewModel
    {
        private readonly Medicine _medicine;

        public string Series { get; set; }
        public string MedicalInstitution { get; set; }
        public string PatientLastName { get; set; }
        public string PatientFirstName { get; set; }
        public string ICD10Code { get; set; }
        public int Quantity { get; set; }
        public string DiscountType { get; set; }
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddMonths(1);
        public List<string> DiscountTypes { get; } = new List<string> { "50%", "Free" };

        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public PrescriptionInputViewModel(Medicine medicine)
        {
            _medicine = medicine;
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Add(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Series) || Quantity <= 0 || Quantity > _medicine.StockQuantity)
            {
                MessageBox.Show("Проверьте введенные данные.");
                return;
            }

            var prescription = new Prescription
            {
                Series = Series,
                MedicalInstitution = MedicalInstitution ?? "Не указано",
                PatientLastName = PatientLastName ?? "Не указано",
                PatientFirstName = PatientFirstName ?? "Не указано",
                ICD10Code = ICD10Code ?? "Не указано",
                Quantity = Quantity,
                DiscountType = DiscountType,
                MedicineId = _medicine.MedicineId,
                PharmacistId = AppContext.CurrentUser.UserId,
                ExpiryDate = ExpiryDate
            };

            string query = "INSERT INTO Prescriptions (Series, MedicalInstitution, PatientLastName, PatientFirstName, ICD10Code, Quantity, DiscountType, MedicineId, PharmacistId, ExpiryDate) VALUES (@Series, @MedicalInstitution, @PatientLastName, @PatientFirstName, @ICD10Code, @Quantity, @DiscountType, @MedicineId, @PharmacistId, @ExpiryDate)";
            SqlParameter[] parameters = {
                new SqlParameter("@Series", prescription.Series),
                new SqlParameter("@MedicalInstitution", prescription.MedicalInstitution),
                new SqlParameter("@PatientLastName", prescription.PatientLastName),
                new SqlParameter("@PatientFirstName", prescription.PatientFirstName),
                new SqlParameter("@ICD10Code", prescription.ICD10Code),
                new SqlParameter("@Quantity", prescription.Quantity),
                new SqlParameter("@DiscountType", prescription.DiscountType),
                new SqlParameter("@MedicineId", prescription.MedicineId),
                new SqlParameter("@PharmacistId", prescription.PharmacistId),
                new SqlParameter("@ExpiryDate", prescription.ExpiryDate)
            };

            DatabaseHelper.ExecuteNonQuery(query, parameters);
            AppContext.CartItems.Add(new CartItem { Medicine = _medicine, Quantity = Quantity, IsByPrescription = true, Prescription = prescription });
            GoBack();
        }

        private void Cancel(object parameter)
        {
            GoBack();
        }

        private void GoBack()
        {
            var window = new MedicineListWindow();
            window.Show();
            Application.Current.Windows.OfType<PrescriptionInputWindow>().FirstOrDefault()?.Close();
        }
    }
}