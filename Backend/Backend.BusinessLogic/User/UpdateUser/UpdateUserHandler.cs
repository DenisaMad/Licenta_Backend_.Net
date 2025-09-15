using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;

namespace Backend.BusinessLogic.User.UpdateUser
{
  public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, UpdateUserResponse>
  {
    private readonly IMongoDataBase mongoDataBase;
    private readonly IHashingService hashingServices;
    public UpdateUserHandler(IMongoDataBase mongoDataBase,IHashingService hashingServices)
    {
      this.mongoDataBase = mongoDataBase;
      this.hashingServices = hashingServices;
    }
    public async Task<UpdateUserResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken)
    {
      var collection = mongoDataBase.GetCollection<Domain.User.User>();
      var filter = Builders<Domain.User.User>.Filter.Eq(x => x.Id, request.Id);
      var user = await collection.Find(filter).FirstOrDefaultAsync();
      user.Email = request.Email;
      user.PhoneNumber = request.PhoneNumber;
      user.Name = request.Name;
      user.Password = this.hashingServices.HashPassword(request.Password,user.Salt);
      await collection.ReplaceOneAsync(filter, user);
      return new UpdateUserResponse
      {
        Message = "Success",
        StatusCode = System.Net.HttpStatusCode.OK
      };
    }
  }
}
