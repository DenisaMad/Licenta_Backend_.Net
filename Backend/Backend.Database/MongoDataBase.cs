using Backend.DataAbstraction.Security;
using MongoDB.Driver;

namespace Backend.Database
{
  public class MongoDataBase : IMongoDataBase
  {
    private readonly string Connection = "mongodb+srv://Admin:23GMxBrkQnf9vta@cluster0.bpvikfn.mongodb.net/";
    public string Database = "LicentaDB";
    private readonly IMongoDatabase mongoDatabase;
    public MongoDataBase()
    {
      var mongoClient = new MongoClient(Connection);
      this.mongoDatabase = mongoClient.GetDatabase(Database);
    }

    public IMongoCollection<T> GetCollection<T>() where T : new()
    {
      return this.mongoDatabase.GetCollection<T>(nameof(T));
    }
  }
}
