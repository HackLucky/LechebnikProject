using System;

namespace LechebnikProject.Models
{
    /// <summary>
    /// Модель рецепта.
    /// </summary>
    public class Prescription
    {
        public int PrescriptionId { get; set; }     // Уникальный идентификатор
        public string Series { get; set; }          // Серия рецепта
        public string MedicalInstitution { get; set; } // Медицинское учреждение
        public string PatientLastName { get; set; } // Фамилия пациента
        public string PatientFirstName { get; set; } // Имя пациента
        public string PatientMiddleName { get; set; } // Отчество пациента
        public string ICD10Code { get; set; }       // Код МКБ-10
        public int Quantity { get; set; }           // Количество
        public string DiscountType { get; set; }    // Тип скидки (50%/Бесплатно)
        public string DoctorLastName { get; set; }  // Фамилия врача
        public string DoctorFirstName { get; set; } // Имя врача
        public string DoctorMiddleName { get; set; } // Отчество врача
        public int MedicineId { get; set; }         // Идентификатор препарата
        public int PharmacistId { get; set; }       // Идентификатор фармацевта
        public DateTime ExpiryDate { get; set; }    // Срок действия
    }
}
