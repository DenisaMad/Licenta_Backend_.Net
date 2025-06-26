using Backend.Database;
using Backend.Domain.User;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet(Name = "Users")]
        public string GetUsers()
        {
            return "Buna ziua";
            
        }

        [HttpPost(Name = "CreateUser")]
        public async Task<string> PostUsers()
        {
            Console.WriteLine("enter name");
            string name = Console.ReadLine();
            Console.WriteLine("enter email");
            string email = Console.ReadLine();
            Console.WriteLine("enter password");
            string password = Console.ReadLine();
            Console.WriteLine("enter Number");
            string PhoneNum = Console.ReadLine();
            Console.WriteLine("enter age");
            int age = int.Parse(Console.ReadLine());

            MongoDataBase mongoDataBase = new MongoDataBase();
            var collection = mongoDataBase.GetUserCollection();
            User user = new User { Email = email, Name =name, Password=password, PhoneNumber=PhoneNum, age=age};
            await collection.InsertOneAsync(user);
            return "User creat cu succes!";
        }

        [HttpPut(Name = "UpdateUser")]
        public string PutUsers()
        {
            return "User actualizat cu succes!";
        }

        [HttpDelete(Name = "DeleteUser")]
        public string DeleteUsers()
        {
            return "User sters cu succes!";
        }
    }

    
}
