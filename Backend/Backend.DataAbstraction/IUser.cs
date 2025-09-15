namespace Backend.DataAbstraction
{
  public enum EUserRole
    {
        /*[Description("Client")]
        client,
        [Description("Admin")]
        admin,
        [Description("Developer")]
        developer
        */
    }
    public interface IUser
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string? Salt { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }

        public EUserRole Role { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    
}

