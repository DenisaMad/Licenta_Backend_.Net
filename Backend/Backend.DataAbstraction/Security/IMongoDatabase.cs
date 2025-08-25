using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DataAbstraction.Security
{
    public interface IMongoDatabase
    {
        public IMongoCollection<User> GetUserCollection();
      
    }
}
