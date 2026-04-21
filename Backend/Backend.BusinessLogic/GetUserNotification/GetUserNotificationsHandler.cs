using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.GetUserNotification
{
    public class GetUserNotificationsHandler : IRequestHandler<GetUserNotificationsRequest, GetUserNotificationsResponse>
    {
        private readonly IMongoCollection<Domain.User.User> _userCollection;

        public GetUserNotificationsHandler(IMongoDataBase mongoDataBase)
        {
            _userCollection = mongoDataBase.GetCollection<Domain.User.User>();
        }

        public async Task<GetUserNotificationsResponse> Handle(GetUserNotificationsRequest request, CancellationToken cancellationToken)
        {
            var user = await _userCollection.Find(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);

            if (user == null)
            {
                return new GetUserNotificationsResponse
                {
                    Notifications = new List<string>(),
                    Medicines = new List<Backend.CommonDomain.MedicineDTO>()
                };
            }

            return new GetUserNotificationsResponse
            {
                Notifications = user.UserNotifications ?? new List<string>(),
                Medicines = user.UserMedicine?.Medicines ?? new List<Backend.CommonDomain.MedicineDTO>()
            };
        }
    }
}
