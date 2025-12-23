using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.CreateMedicine
{
    public class CreateMedicineValidator:AbstractValidator<CreateMedicineRequest>
    {
        public CreateMedicineValidator() {
            this.RuleFor(x => x.Name).NotEmpty().WithMessage("Medicine name is required.")
               .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");

        }
    }
}
