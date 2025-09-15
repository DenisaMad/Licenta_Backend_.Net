using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;

namespace Backend.BusinessLogic.User.GetUsers
{
  public class GetUsersHandler : IRequestHandler<GetUsersRequest, GetUsersResponse>
  {
    private readonly IMongoDataBase mongoDataBase;
    public GetUsersHandler(IMongoDataBase mongoDataBase)
    {
      this.mongoDataBase = mongoDataBase;
    }
    public async Task<GetUsersResponse> Handle(GetUsersRequest request, CancellationToken cancellationToken)
    {
      var collection = mongoDataBase.GetCollection<Domain.User.User>();
      var filter = Builders<Domain.User.User>.Filter.Empty;
      var usersFromDB = await collection.Find(filter).ToListAsync();

      return new GetUsersResponse
      {
        Message = "Success",
        StatusCode = System.Net.HttpStatusCode.OK,
        Users = usersFromDB,
      };
    }
  }
}
