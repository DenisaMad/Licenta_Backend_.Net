using Backend.Domain.MedicineDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.CreateMedicine
{
    public class CreateMedicineResponse
    {
        public string MedicineName { get; set; } = string.Empty;
        public Medicine? MedicineData { get; set; }
        public bool Success { get; set; }
    }
}
