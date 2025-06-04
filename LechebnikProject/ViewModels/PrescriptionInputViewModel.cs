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

        public List<string> DiscountTypes { get; } = new List<string> { "50%", "Бесплатно" };

        //public Prescription Prescription { get; set; } = new Prescription();
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

        public ICommand AddCommand { get; }
        public ICommand CancelCommand { get; }

        public PrescriptionInputViewModel(Medicine medicine)
        {
            _medicine = medicine;
            SelectedMedicine = medicine;
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(o => WindowManager.ShowWindow<MedicineListWindow>());

        }

        private void Add(object parameter)
        {
            if (string.IsNullOrWhiteSpace(Series) ||
                string.IsNullOrWhiteSpace(MedicalInstitution) ||
                string.IsNullOrWhiteSpace(PatientLastName) ||
                string.IsNullOrWhiteSpace(PatientFirstName) ||
                string.IsNullOrWhiteSpace(ICD10Code) ||
                Quantity <= 0 ||
                string.IsNullOrWhiteSpace(DiscountType) ||
                string.IsNullOrWhiteSpace(DoctorLastName) ||
                string.IsNullOrWhiteSpace(DoctorFirstName) ||
                ExpiryDate == default)
            {
                MessageBox.Show("Заполните все обязательные поля.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Quantity > _medicine.StockQuantity)
            {
                MessageBox.Show("Требуемое количество препарата превышает количество имеющихся на складе.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (Quantity > 100)
            {
                MessageBox.Show("Нельзя приобрести больше 100 препаратов.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            const string checkSql = "SELECT COUNT(*) FROM Prescriptions WHERE Series = @Series";
            var checkPrm = new[] { new SqlParameter("@Series", Series) };
            int exists = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkSql, checkPrm));
            if (exists > 0)
            {
                MessageBox.Show("Рецепт с такой серией уже зарегистрирован.", "Предупреждение.", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var prescription = new Prescription
            {
                Series = Series,
                MedicalInstitution = MedicalInstitution,
                PatientLastName = PatientLastName,
                PatientFirstName = PatientFirstName,
                PatientMiddleName = string.IsNullOrWhiteSpace(PatientMiddleName) ? null : PatientMiddleName,
                ICD10Code = ICD10Code,
                Quantity = Quantity,
                DiscountType = DiscountType,
                DoctorLastName = DoctorLastName,
                DoctorFirstName = DoctorFirstName,
                DoctorMiddleName = string.IsNullOrWhiteSpace(DoctorMiddleName) ? null : DoctorMiddleName,
                MedicineId = SelectedMedicine.MedicineId,
                PharmacistId = AppContext.CurrentUser.UserId,
                ExpiryDate = ExpiryDate
            };

            string insertPrescriptionQuery = @"
                INSERT INTO Prescriptions 
                    (Series, MedicalInstitution, PatientLastName, PatientFirstName, PatientMiddleName, 
                     ICD10Code, Quantity, DiscountType, DoctorLastName, DoctorFirstName, DoctorMiddleName, 
                     MedicineId, PharmacistId, ExpiryDate)
                OUTPUT INSERTED.PrescriptionId
                VALUES 
                    (@Series, @MedicalInstitution, @PatientLastName, @PatientFirstName, @PatientMiddleName,
                     @ICD10Code, @Quantity, @DiscountType, @DoctorLastName, @DoctorFirstName, @DoctorMiddleName,
                     @MedicineId, @PharmacistId, @ExpiryDate)";

            var parameters = new[]
            {
                new SqlParameter("@Series", prescription.Series),
                new SqlParameter("@MedicalInstitution", prescription.MedicalInstitution),
                new SqlParameter("@PatientLastName", prescription.PatientLastName),
                new SqlParameter("@PatientFirstName", prescription.PatientFirstName),
                new SqlParameter("@PatientMiddleName", (object)prescription.PatientMiddleName ?? DBNull.Value),
                new SqlParameter("@ICD10Code", prescription.ICD10Code),
                new SqlParameter("@Quantity", prescription.Quantity),
                new SqlParameter("@DiscountType", prescription.DiscountType),
                new SqlParameter("@DoctorLastName", prescription.DoctorLastName),
                new SqlParameter("@DoctorFirstName", prescription.DoctorFirstName),
                new SqlParameter("@DoctorMiddleName", (object)prescription.DoctorMiddleName ?? DBNull.Value),
                new SqlParameter("@MedicineId", prescription.MedicineId),
                new SqlParameter("@PharmacistId", prescription.PharmacistId),
                new SqlParameter("@ExpiryDate", prescription.ExpiryDate)
            };

            try
            {
                int prescriptionId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(insertPrescriptionQuery, parameters));

                decimal basePrice = SelectedMedicine.Price * Quantity;
                decimal finalPrice = basePrice;

                if (DiscountType == "50%")
                {
                    finalPrice = basePrice * 0.5m;
                    MessageBox.Show($"Препарат с рецептом добавлен в корзину по скидке 50%. Итого: {finalPrice} руб.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Добавляем в глобальный контекст корзины
                    AppContext.CartItems.Add(new CartItem
                    {
                        Medicine = SelectedMedicine,
                        Quantity = Quantity,
                        IsByPrescription = true,
                        PrescriptionId = prescriptionId
                    });

                    WindowManager.ShowWindow<MedicineListWindow>();
                }
                else if (DiscountType == "Бесплатно")
                {
                    // Записываем заказ с нулевой суммой и 100% скидкой
                    string insertOrderSql = @"
                        INSERT INTO Orders 
                            (UserId, OrderDate, TotalAmount, PaymentMethod, DiscountApplied, DiscountPercentage)
                        VALUES 
                            (@UserId, GETDATE(), 0, 'Бесплатно', 1, 100)";

                    var orderParams = new[] { new SqlParameter("@UserId", AppContext.CurrentUser.UserId) };
                    DatabaseHelper.ExecuteNonQuery(insertOrderSql, orderParams);

                    MessageBox.Show("Препарат предоставлен бесплатно по рецепту. Заказ оформлен и занесён в отчёт.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    WindowManager.ShowWindow<MedicineListWindow>();
                }
            }
            catch (SqlException ex)
            {
                Logger.LogError("Ошибка при добавлении рецепта.", ex);
                MessageBox.Show("Ошибка сохранения рецепта. Попробуйте ещё раз или обратитесь к администратору.", "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}