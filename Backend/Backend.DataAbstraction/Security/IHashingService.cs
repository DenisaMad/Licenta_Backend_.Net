namespace Backend.DataAbstraction.Security
{
  public interface IHashingService
  {
    public string HashPassword(string password, string salt);

    public bool VerifyPassword(string password, string salt, string hashed);

    public string GenerateSalt();

    public string GenerateRandomCode();//THIS SHOULD GENERATE A CODE WITH A PRIVATE KEY AND THE CODE SHOULD CONTAIN AN EXPIRE TIME
    public bool ValidCode(string code);//THIS SHOULD VALIDATE IF A CODE WAS GENERATEDBY OUR BACKEND ( USING THE PRIVATE KEY) AND CHECKS THE EXPIRE TIME TOO

  }
}
