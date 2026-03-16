using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.GetUserNotification
{
    public class GetUserNotificationsRequest : IRequest<GetUserNotificationsResponse>
    {
        public string UserId { get; set; }
    }
}
