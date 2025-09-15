namespace Backend.DataAbstraction
{
  public interface IEmailSender
  {
    public string SendVerificationCode(string code, string email);//gmail -> password dedicated (application passwords)
  }
}
