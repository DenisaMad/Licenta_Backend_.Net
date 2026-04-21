using Backend.BusinessLogic.GetUserNotification;
using Backend.BusinessLogic.User.ActivateUserAccountViaCode;
using Backend.BusinessLogic.User.CreateUser;
using Backend.BusinessLogic.User.DeleteUser;
using Backend.BusinessLogic.User.GetUsers;
using Backend.BusinessLogic.User.LoginUser;
using Backend.BusinessLogic.User.UpdateUser;
using Backend.DataAbstraction.Security;
using Backend.Database;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {

        private readonly IMediator mediator;

        public UsersController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet(Name = "Users")]
        public async Task<IActionResult> GetUsers()
        {
            GetUsersRequest request = new GetUsersRequest();
            GetUsersResponse response = await this.mediator.Send(request);
            return this.Ok(response);
        }

        [HttpPost(Name = "CreateUser")]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser(BusinessLogic.User.LoginUser.LoginUserRequest request)
        {
            LoginUserResponse response = await this.mediator.Send(request);
            return this.Ok(response);
        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] BusinessLogic.User.RefreshToken.RefreshTokenRequest request)
        {
            var response = await this.mediator.Send(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return this.Ok(response);
        }

        [HttpPut("ActivateAccount")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateUserAccountViaCode(BusinessLogic.User.ActivateUserAccountViaCode.ActivateUserAccountViaCodeRequest request)
        {
            ActivateUserAccountViaCodeResponse response = await this.mediator.Send(request);
            return this.Ok(response);
        }

        [HttpGet("notifications/{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            var request = new GetUserNotificationsRequest { UserId = userId };
            var response = await this.mediator.Send(request);
            return Ok(response);
        }

        [HttpPut("MarkMedicineTaken")]
        public async Task<IActionResult> MarkMedicineTaken([FromBody] Backend.BusinessLogic.User.MarkMedicineTaken.MarkMedicineTakenRequest request)
        {
            var response = await this.mediator.Send(request);
            if (!response.Success)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return NotFound(response);
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}

