using Lechebnik.ViewModels;
using LechebnikProject.Helpers;
using LechebnikProject.Models;
using LechebnikProject.Views;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace LechebnikProject.ViewModels
{
    public class PrescriptionInputViewModel : BaseViewModel
    {
        private readonly Medicine _medicine;

        public Prescription Prescription { get; set; } = new Prescription();
        public Medicine SelectedMedicine { get; set; }
        public string QuantityText { get; set; }
        public string SelectedDiscountType { get; set; }

        public string Series { get; set; }
        public string MedicalInstitution { get; set; }
        public string PatientLastName { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientMiddleName { get; set; }
        public string ICD10Code { get; set; }
        public int Quantity { get; set; }
        public string DiscountType { get; set; }
        public string DoctorLastName { get; set; }
        public string DoctorFirstName { get; set; }
        public string DoctorMiddleName { get; set; }
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddMonths(1);
        public List<string> DiscountTypes { get; } = new List<string> { "50%", "Бесплатно" };

        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public PrescriptionInputViewModel(Medicine medicine)
        {
            _medicine = medicine;
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(o => WindowManager.ShowWindow<MedicineListWindow>());

        }

        private void Add(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Series) || string.IsNullOrWhiteSpace(MedicalInstitution) || string.IsNullOrWhiteSpace(PatientLastName) || string.IsNullOrWhiteSpace(PatientFirstName)
                || string.IsNullOrWhiteSpace(ICD10Code) || Quantity <= 0 || string.IsNullOrWhiteSpace(DiscountType) || string.IsNullOrWhiteSpace(DoctorLastName) || string.IsNullOrWhiteSpace(DoctorFirstName))
            {
                MessageBox.Show("Проверьте введенные данные. Все обязательные поля должны быть заполнены.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Quantity > _medicine.StockQuantity)
            {
                MessageBox.Show("Требуемое количество препарата превышает количество имеющихся на складе.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var prescription = new Prescription
            {
                Series = Series,
                MedicalInstitution = MedicalInstitution ?? "Не указано",
                PatientLastName = PatientLastName ?? "Не указано",
                PatientFirstName = PatientFirstName ?? "Не указано",
                PatientMiddleName = PatientMiddleName ?? "Не указано",
                ICD10Code = ICD10Code ?? "Не указано",
                Quantity = Quantity,
                DiscountType = DiscountType,
                DoctorLastName = DoctorLastName,
                DoctorFirstName = DoctorFirstName,
                DoctorMiddleName = DoctorMiddleName,
                MedicineId = _medicine.MedicineId,
                PharmacistId = AppContext.CurrentUser.UserId,
                ExpiryDate = ExpiryDate
            };

            const string checkSql = "SELECT COUNT(*) FROM Prescriptions WHERE Series = @Series";
            var checkPrm = new[] { new SqlParameter("@Series", Series) };
            int exists = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkSql, checkPrm));
            if (exists > 0)
            {
                MessageBox.Show("Рецепт с такой серией уже зарегистрирован.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string insertPrescriptionQuery = @"INSERT INTO Prescriptions (Series, MedicalInstitution, PatientLastName, PatientFirstName, PatientMiddleName,
                    ICD10Code, Quantity, DiscountType, DoctorLastName, DoctorFirstName, DoctorMiddleName, MedicineId, PharmacistId, ExpiryDate)
                OUTPUT INSERTED.PrescriptionId
                VALUES (@Series, @MedicalInstitution, @PatientLastName, @PatientFirstName, @PatientMiddleName, @ICD10Code, @Quantity,
                        @DiscountType, @DoctorLastName, @DoctorFirstName, @DoctorMiddleName, @MedicineId, @PharmacistId, @ExpiryDate)";
            var parameters = new[]
            {
                new SqlParameter("@Series", Prescription.Series),
                new SqlParameter("@MedicalInstitution", Prescription.MedicalInstitution),
                new SqlParameter("@PatientLastName", Prescription.PatientLastName),
                new SqlParameter("@PatientFirstName", Prescription.PatientFirstName),
                new SqlParameter("@PatientMiddleName", (object)Prescription.PatientMiddleName ?? DBNull.Value),
                new SqlParameter("@ICD10Code", Prescription.ICD10Code),
                new SqlParameter("@Quantity", Quantity),
                new SqlParameter("@DiscountType", SelectedDiscountType),
                new SqlParameter("@DoctorLastName", Prescription.DoctorLastName),
                new SqlParameter("@DoctorFirstName", Prescription.DoctorFirstName),
                new SqlParameter("@DoctorMiddleName", (object)Prescription.DoctorMiddleName ?? DBNull.Value),
                new SqlParameter("@MedicineId", SelectedMedicine.MedicineId),
                new SqlParameter("@PharmacistId", AppContext.CurrentUser.UserId),
                new SqlParameter("@ExpiryDate", Prescription.ExpiryDate)
            };
            try
            {
                int prescriptionId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertPrescriptionQuery, parameters));

                decimal basePrice = SelectedMedicine.Price * Quantity;
                decimal finalPrice = basePrice;
                bool isPaid = false;

                if (SelectedDiscountType == "50%")
                {
                    finalPrice = basePrice * 0.5m;
                    MessageBox.Show($"Препарат с рецептом добавлен в корзину по скидке 50%. Итого: {finalPrice} руб.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (SelectedDiscountType == "Бесплатно")
                {
                    isPaid = true;
                    MessageBox.Show("Препарат предоставлен бесплатно по рецепту. Заказ оформлен и занесён в отчёт.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                    string insertOrderSql = @"
                    INSERT INTO Orders (UserId, OrderDate, TotalAmount, PaymentMethod, DiscountApplied, DiscountPercentage)
                    VALUES (@UserId, GETDATE(), 0, 'Бесплатно', 1, 100)";
                    var orderParams = new[]
                    {
                    new SqlParameter("@UserId", AppContext.CurrentUser.UserId)
                };
                    DatabaseHelper.ExecuteNonQuery(insertOrderSql, orderParams);

                    WindowManager.ShowWindow<MedicineListWindow>();
                    return;
                }

                AppContext.CartItems.Add(new CartItem
                {
                    Medicine = SelectedMedicine,
                    Quantity = Quantity,
                    IsByPrescription = true,
                    PrescriptionId = prescriptionId
                });

                WindowManager.ShowWindow<MedicineListWindow>();
            }
            catch (SqlException ex)
            {
                Logger.LogError("Ошибка при добавлении рецепта.", ex);
                MessageBox.Show("Ошибка сохранения рецепта. Попробуйте ещё раз или обратитесь к администратору.", "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}