using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;

namespace Backend.BusinessLogic.User.DeleteUser
{
  public class DeleteUserHandler : IRequestHandler<DeleteUserRequest, DeleteUserResponse>
  {
    private readonly IMongoDataBase mongoDataBase;
    public DeleteUserHandler(IMongoDataBase mongoDataBase)
    {
      this.mongoDataBase = mongoDataBase;
    }

    public async Task<DeleteUserResponse> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
      var collection = mongoDataBase.GetCollection<Domain.User.User>();
      var filter = Builders<Domain.User.User>.Filter.Eq(x => x.Id, request.Id);
      var deleted = await collection.DeleteOneAsync(filter);
      return new DeleteUserResponse
      {
        Message = "Deleted",
        StatusCode = System.Net.HttpStatusCode.OK
      };
    }
  }
}
