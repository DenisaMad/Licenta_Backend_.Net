using Backend.DataAbstraction.Database;
using Backend.DataAbstraction.Security;
using MongoDB.Driver;

namespace Backend.Database
{
  public class MongoDataBase : IMongoDataBase
  {
    
    private readonly IMongoDatabase mongoDatabase;
    public MongoDataBase(IDatabaseSettings settings)
    {
      var mongoClient = new MongoClient(settings.ConnectionString);
      this.mongoDatabase = mongoClient.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>() where T : new()
    {
      return this.mongoDatabase.GetCollection<T>(typeof(T).Name);
    }
  }
}
