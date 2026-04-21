using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;
using System.Net;

namespace Backend.BusinessLogic.User.MarkMedicineTaken
{
    public class MarkMedicineTakenHandler : IRequestHandler<MarkMedicineTakenRequest, MarkMedicineTakenResponse>
    {
        private readonly IMongoCollection<Domain.User.User> _userCollection;

        public MarkMedicineTakenHandler(IMongoDataBase mongoDataBase)
        {
            _userCollection = mongoDataBase.GetCollection<Domain.User.User>();
        }

        public async Task<MarkMedicineTakenResponse> Handle(MarkMedicineTakenRequest request, CancellationToken cancellationToken)
        {
            var user = await _userCollection.Find(u => u.Id == request.UserId).FirstOrDefaultAsync(cancellationToken);
            if (user == null || user.UserMedicine == null || user.UserMedicine.Medicines == null)
            {
                return new MarkMedicineTakenResponse
                {
                    Success = false,
                    Message = "User or Medicines not found.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            var medicine = user.UserMedicine.Medicines.FirstOrDefault(m => m.MedicineName == request.MedicineName);
            if (medicine == null)
            {
                return new MarkMedicineTakenResponse
                {
                    Success = false,
                    Message = "Medicine not found.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            bool changed = false;

            if (request.TimeOfDay.Equals("Morning", StringComparison.OrdinalIgnoreCase))
            {
                if (medicine.TakenMorning != request.IsTaken) { medicine.TakenMorning = request.IsTaken; changed = true; }
            }
            else if (request.TimeOfDay.Equals("Noon", StringComparison.OrdinalIgnoreCase))
            {
                if (medicine.TakenNoon != request.IsTaken) { medicine.TakenNoon = request.IsTaken; changed = true; }
            }
            else if (request.TimeOfDay.Equals("Evening", StringComparison.OrdinalIgnoreCase))
            {
                if (medicine.TakenEvening != request.IsTaken) { medicine.TakenEvening = request.IsTaken; changed = true; }
            }
            else
            {
                return new MarkMedicineTakenResponse
                {
                    Success = false,
                    Message = "Invalid TimeOfDay. Use Morning, Noon, or Evening.",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            if (changed)
            {
                var update = Builders<Domain.User.User>.Update.Set(u => u.UserMedicine, user.UserMedicine);
                await _userCollection.UpdateOneAsync(u => u.Id == request.UserId, update, cancellationToken: cancellationToken);
            }

            return new MarkMedicineTakenResponse
            {
                Success = true,
                Message = "Medicine marked as taken successfully.",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
