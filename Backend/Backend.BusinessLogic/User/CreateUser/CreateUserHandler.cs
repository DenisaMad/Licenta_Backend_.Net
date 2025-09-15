using Backend.DataAbstraction;
using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;

namespace Backend.BusinessLogic.User.CreateUser
{
  public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
  {
    private readonly IMongoDataBase database;
    private readonly IHashingService hashingServices;
    private readonly IEmailSender emailSender;

    public CreateUserHandler(IMongoDataBase mongoDataBase, IHashingService hashingServices, IEmailSender emailSender)
    {
      this.database = mongoDataBase;
      this.hashingServices = hashingServices;
      this.emailSender = emailSender;
    }
    public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
    {
      var collection = database.GetCollection<Domain.User.User>();
      var filter = Builders<Domain.User.User>.Filter.Eq(x => x.Email, request.Email);
      var userFromDB = await collection.Find(filter).FirstOrDefaultAsync();
      if (userFromDB == null)
      {
        userFromDB = new Domain.User.User();
        userFromDB.Email = request.Email;
        userFromDB.Salt = this.hashingServices.GenerateSalt();
        userFromDB.Password = this.hashingServices.HashPassword(request.Password, userFromDB.Salt);
        await collection.InsertOneAsync(userFromDB);
        this.emailSender.SendVerificationCode(this.hashingServices.GenerateRandomCode(), userFromDB.Email);
        return new CreateUserResponse
        {
          Message = "Success",
          StatusCode = System.Net.HttpStatusCode.Created
        };
      }
      return new CreateUserResponse
      {
        Message = "Fail",
        StatusCode = System.Net.HttpStatusCode.BadRequest
      };
    }
  }
}
