using Backend.CommonDomain.Users;
using Backend.DataAbstraction.Security;
using Backend.Database;
using Backend.Domain.User;
using Backend.Services;
using Backend.Services.Security;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        IHashingServices hashingServices;
        IAuthtentificationServices authtentificationServices;

        public UsersController(IHashingServices hashingServices, IAuthtentificationServices authtentificationServices, MongoDataBase mongoDataBase)
        {
            this.hashingServices = hashingServices;
            this.authtentificationServices = authtentificationServices;
            this.mongoDataBase = mongoDataBase;
        }

        MongoDataBase mongoDataBase = new MongoDataBase();
        [HttpGet(Name = "Users")]
        public async Task<List<User>> GetUsers()
        {

            var collection = mongoDataBase.GetUserCollection();
            var filter = Builders<User>.Filter.Empty;
            var usersFromDB = await collection.Find(filter).ToListAsync();

            return usersFromDB;

        }

        [HttpPost(Name = "CreateUser")]
        public async Task<string> PostUsers(User user)
        {
            var collection = mongoDataBase.GetUserCollection();
            var filter = Builders<User>.Filter.Eq(x => x.Email, user.Email);
            var userFromDB = await collection.Find(filter).FirstOrDefaultAsync();

            HashingService hashingPassword = new HashingService();
            user.Salt = hashingPassword.GenerateSalt();
            user.Password = hashingPassword.HashPassword(user.Password, user.Salt);
            if (userFromDB == null)
            {
                await collection.InsertOneAsync(user);
                return "User creat cu succes!";
            }
            else return "Email ul este deja folosit!!!";
        }

        [HttpPut(Name = "UpdateUser")]
        public async Task<string> PutUsers(User user)
        {
            var collection = mongoDataBase.GetUserCollection();
            var filter = Builders<User>.Filter.Eq(x => x.Id, user.Id);
            await collection.ReplaceOneAsync(filter, user);
            return "User updated!";
        }

        [HttpDelete(Name = "DeleteUser")]
        public async Task<string> DeleteUsers(string Id)
        {
            var collection = mongoDataBase.GetUserCollection();
            var filter = Builders<User>.Filter.Eq(x => x.Id, Id);
            await collection.DeleteOneAsync(filter);
            return "User sters cu succes!";
        }

        [HttpGet("GetUserByEmail/{email}")]
        public async Task<User> GetUsersByEmail(string email)
        {

            var collection = mongoDataBase.GetUserCollection();
            var filter = Builders<User>.Filter.Eq(x => x.Email, email);
            var user = await collection.Find(filter).FirstOrDefaultAsync();
            return user;

        }

        [HttpPost("LoginUser")]
        public async Task<LoginResponse> LoginUser(LoginUserRequest loginUserRequest)
        {
            var collection = mongoDataBase.GetUserCollection();
            var filter = Builders<User>.Filter.Eq(x => x.Email, loginUserRequest.Email);
            var userFromDB = await collection.Find(filter).FirstOrDefaultAsync();

            if (userFromDB == null)
            {
                return new LoginResponse
                {
                    AccessToken = " ",
                    RefreshToken = " "
                };
            }

            HashingService hashingService = new HashingService();
            bool isPasswordValid = hashingService.VerifyPassword(loginUserRequest.Password, userFromDB.Salt, userFromDB.Password);
            var authentificationServices = new AuthtentificationService();
            if (isPasswordValid)
            {
                string accessToken= authentificationServices.GenerateAccessToken(loginUserRequest.Email, userFromDB.Name, userFromDB.Role);
                string refreshToken = authentificationServices.GenerateRefreshToken();


                //creez o clasa cu 2 proprietati access si refresh token si ma folosesc de ea ca sa ii atribui access token si refresh token si sa o returnez
                //salvam in user tokenele create

                userFromDB.AccessToken = accessToken;
                userFromDB.RefreshToken = refreshToken;

                var updateFilter = Builders<User>.Filter.Eq(x => x.Id, userFromDB.Id);
                await collection.ReplaceOneAsync(updateFilter, userFromDB);

                return new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
            }
            else
            {
                return new LoginResponse
                {
                    AccessToken = " ",
                    RefreshToken = " "
                };
            }

          
        }





        
    }

}
