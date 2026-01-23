using Backend.BusinessLogic.User.CreateUser;
using Backend.CommonDomain;
using Backend.DataAbstraction.BearerTokens;
using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.MedicineDTO
{
    public class MedicineHandler : IRequestHandler<MedicineRequest, MedicineResponse>
    {
        private readonly IMongoDataBase mongoDataBase;
        public MedicineHandler(IMongoDataBase mongoDataBase)
        {
            this.mongoDataBase = mongoDataBase;

        }
        public async Task<MedicineResponse> Handle(MedicineRequest request, CancellationToken cancellationToken)
        {
            var collection = mongoDataBase.GetCollection<Domain.User.User>();
            var filter = Builders<Domain.User.User>.Filter.Eq(x => x.Id, request.Id);
            var userFromDB = await collection.Find(filter).FirstOrDefaultAsync();
            userFromDB.UserMedicine = new UserMedicineDTO
            {
                PatientName = request.PatientName,
                DoctorName = request.DoctorName,
                Date = request.Date,
                Medicines = request.Medicines

            };
            await mongoDataBase.GetCollection<Domain.User.User>().ReplaceOneAsync(Builders<Domain.User.User>.Filter.Eq(x => x.Id, request.Id), userFromDB, cancellationToken: cancellationToken);
            return new MedicineResponse { Success = true};
        }

    }
}
