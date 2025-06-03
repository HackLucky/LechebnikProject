using System;

namespace LechebnikProject.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
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
        public int MedicineId { get; set; }
        public int PharmacistId { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
