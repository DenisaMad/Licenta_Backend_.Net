using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.CommonDomain
{
    public  class UserMedicineDTO
    {
        public string Id { get; set; }
        public List<MedicineDTO> Medicines { get; set; }
        public string DoctorName { get; set; }

        public string PatientName { get; set; }
        public DateTime Date { get; set; }
    }
}
