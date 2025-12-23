using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.MedicineDomain
{
    public class MedicineLabel
    {
        public string MedicineName {  get; set; }
        public Medicine Labels {  get; set; }

        public Prescription Prescription { get; set; } 

    }
}
