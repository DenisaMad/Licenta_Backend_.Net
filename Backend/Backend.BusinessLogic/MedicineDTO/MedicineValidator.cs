using Backend.BusinessLogic.CreateMedicine;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.MedicineDTO
{
    public class MedicineValidator : AbstractValidator<MedicineRequest>
    {
        public MedicineValidator() {
            this.RuleFor(x => x.DoctorName).NotEmpty().WithMessage("Doctor name is required.")
                   .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");
            this.RuleFor(x => x.PatientName).NotEmpty().WithMessage("Patient name is required.")
                   .MinimumLength(2).WithMessage("Name must be at least 2 characters long.");
            this.RuleFor(x => x.Date).NotNull().WithMessage("Date is required.");
        }
    }
}
