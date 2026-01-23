using Backend.CommonDomain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.MedicineDTO
{
    public class MedicineRequest: UserMedicineDTO, IRequest<MedicineResponse>
    {

    }
}
