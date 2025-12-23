using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.ActivateUserAccountViaCode
{
  public class ActivateUserAccountViaCodeValidator:AbstractValidator<ActivateUserAccountViaCodeRequest>
  {
        public ActivateUserAccountViaCodeValidator() {

            this.RuleFor(request => request.Code).NotEmpty();
        }
    }
}
