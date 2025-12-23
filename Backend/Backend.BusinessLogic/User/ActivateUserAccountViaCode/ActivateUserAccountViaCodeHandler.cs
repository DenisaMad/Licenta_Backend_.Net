using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.BusinessLogic.User.ActivateUserAccountViaCode
{
    public class ActivateUserAccountViaCodeHandler : IRequestHandler<ActivateUserAccountViaCodeRequest, ActivateUserAccountViaCodeResponse>
    {
        private readonly IMongoDataBase database;

        public ActivateUserAccountViaCodeHandler(IMongoDataBase mongoDataBase)
        {
            this.database= mongoDataBase;

        }
        public async Task<ActivateUserAccountViaCodeResponse> Handle(ActivateUserAccountViaCodeRequest request, CancellationToken cancellationToken)
        {
            var collection = database.GetCollection<Domain.User.User>();
            var filter = Builders<Domain.User.User>.Filter.Eq(x => x.ActiveAccountCode, request.Code);
            var userFromDB = await collection.Find(filter).FirstOrDefaultAsync();
            if (userFromDB == null)
            {
                return new ActivateUserAccountViaCodeResponse
                {

                    Message = "Invalid code",
                    Success = false

                };
                
            }
            userFromDB.ActiveAccountCode = null;
            userFromDB.ActiveAccount = true;
            await collection.ReplaceOneAsync(filter,userFromDB);
            return new ActivateUserAccountViaCodeResponse
            {
                Message = "Cont activat cu succes!",
                Success = true

            };
        }

    }
}
