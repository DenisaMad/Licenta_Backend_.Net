using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.MedicineDomain
{
    public class Prescription
    {
        public int NumberOfCapsules {  get; set; }
        public int NumberOfTimesPerDay {  get; set; }
    }
}
