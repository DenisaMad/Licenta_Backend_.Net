using Backend.Domain.User;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Database
{
    public class MongoDataBase
    {
        private readonly string Connection = "mongodb+srv://Admin:23GMxBrkQnf9vta@cluster0.bpvikfn.mongodb.net/";
        public string Database = "LicentaDB";
        public string UsersCollection = "Users";

        public IMongoCollection<User> GetUserCollection() { 
            IMongoClient client = new MongoClient(Connection);
            IMongoDatabase database = client.GetDatabase(Database);
            IMongoCollection<User> collection = database.GetCollection<User>(UsersCollection);
            return collection;
        }
    }
}
