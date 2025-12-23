using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.ActivateUserAccountViaCode
{
  public class ActivateUserAccountViaCodeRequest: IRequest<ActivateUserAccountViaCodeResponse>
  {
    public string Code { get; set; }
  }
}
