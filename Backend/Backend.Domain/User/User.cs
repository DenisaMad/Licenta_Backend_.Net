using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Domain.User
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } = null;
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Salt { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }

        public EUserRole Role {get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken {  get; set; }
    }
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
