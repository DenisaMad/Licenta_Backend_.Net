using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.CreateMedicine
{
    public class CreateMedicineRequest: IRequest<CreateMedicineResponse>
    {
        public string Name { get; set; } = string.Empty;
    }
}
