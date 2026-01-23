using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.CommonDomain
{
    public class Dto
    {
        public string Name { get; set; }
        public int CountMorning { get; set; }
        public int CountAfterNon { get; set; }
        public int CountNight { get; set; }
    }
}
