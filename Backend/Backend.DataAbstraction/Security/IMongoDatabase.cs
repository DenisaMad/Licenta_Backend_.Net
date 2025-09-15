using MongoDB.Driver;

namespace Backend.DataAbstraction.Security
{
  public interface IMongoDataBase
  {
    public IMongoCollection<T> GetCollection<T>()
   where T : new();

  }
}
