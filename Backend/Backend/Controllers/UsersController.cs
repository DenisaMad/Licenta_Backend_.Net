using Backend.BusinessLogic.User.CreateUser;
using Backend.BusinessLogic.User.DeleteUser;
using Backend.BusinessLogic.User.GetUsers;
using Backend.BusinessLogic.User.LoginUser;
using Backend.BusinessLogic.User.UpdateUser;
using Backend.DataAbstraction.Security;
using Backend.Database;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class UsersController : ControllerBase
  {

    private readonly IMediator mediator;

    public UsersController(IMediator mediator)
    {
      this.mediator = mediator;
    }

    MongoDataBase mongoDataBase = new MongoDataBase();
    [HttpGet(Name = "Users")]
    public async Task<IActionResult> GetUsers()
    {
      GetUsersRequest request = new GetUsersRequest();
      GetUsersResponse response = await this.mediator.Send(request);
      return this.Ok(response);
    }

    [HttpPost(Name = "CreateUser")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
      CreateUserResponse response = await this.mediator.Send(request);
      return this.Ok(response);
    }

    [HttpPut(Name = "UpdateUser")]
    public async Task<IActionResult> PutUsers(UpdateUserRequest request)
    {
      UpdateUserResponse response = await this.mediator.Send(request);
      return this.Ok(response);
    }

    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
      DeleteUserRequest request = new DeleteUserRequest { Id = id };
      DeleteUserResponse response = await this.mediator.Send(request);
      return this.Ok(response);
    }

    [HttpPost("LoginUser")]
    public async Task<IActionResult> LoginUser(BusinessLogic.User.LoginUser.LoginUserRequest request)
    {
      LoginUserResponse response = await this.mediator.Send(request);
      return this.Ok(response);
    }
  }
}
