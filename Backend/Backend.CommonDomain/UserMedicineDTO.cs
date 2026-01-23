namespace Backend.CommonDomain
{
    public  class UserMedicineDTO
    {
        public List<MedicineDTO> Medicines { get; set; }
        public string DoctorName { get; set; }

        public string PatientName { get; set; }
        public DateTime Date { get; set; }
    }
}
