using Backend.DataAbstraction;
using Backend.DataAbstraction.Security;
using MediatR;
using MongoDB.Driver;

namespace Backend.BusinessLogic.User.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
    {
        private readonly IMongoDataBase database;
        private readonly IHashingService hashingServices;
        private readonly IEmailSender emailSender;

        public CreateUserHandler(IMongoDataBase mongoDataBase, IHashingService hashingServices, IEmailSender emailSender)
        {
            this.database = mongoDataBase;
            this.hashingServices = hashingServices;
            this.emailSender = emailSender;
        }
        public async Task<CreateUserResponse> Handle(CreateUserRequest request, CancellationToken cancellationToken)
        {
            var collection = database.GetCollection<Domain.User.User>();
            var filter = Builders<Domain.User.User>.Filter.Eq(x => x.Email, request.Email);
            var userFromDB = await collection.Find(filter).FirstOrDefaultAsync();
            if (userFromDB == null)
            {
                string code = this.hashingServices.GenerateRandomCode();
                userFromDB = new Domain.User.User();
                userFromDB.Email = request.Email;
                userFromDB.Name = request.Name;
                userFromDB.Salt = this.hashingServices.GenerateSalt();
                userFromDB.Password = this.hashingServices.HashPassword(request.Password, userFromDB.Salt);
                userFromDB.ActiveAccountCode = code;
                await collection.InsertOneAsync(userFromDB);

                string emailBody = $@"
        <html>
            <body style='font-family: Arial, sans-serif; background-color: #F8FAFC; padding: 20px; margin: 0;'>
                <div style='background-color: #ffffff; padding: 30px; border-radius: 16px; box-shadow: 0 10px 25px rgba(0,0,0,0.05); max-width: 500px; margin: 0 auto; border: 1px solid #E2E8F0;'>
                    <div style='text-align: center; margin-bottom: 25px;'>
                        <h2 style='color: #0F172A; margin: 0; font-size: 26px; font-weight: 800;'>Verify your email</h2>
                    </div>
                    <p style='color: #334155; font-size: 16px; line-height: 1.6; margin-bottom: 15px;'>Hi <strong>{userFromDB.Name}</strong>,</p>
                    <p style='color: #475569; font-size: 16px; line-height: 1.6; margin-bottom: 25px;'>Thank you for creating an account with us. Please use the verification code below to activate your account and get started.</p>
                    <div style='text-align: center; margin: 35px 0;'>
                        <span style='font-size: 32px; font-weight: 700; color: #0F172A; background-color: #F1F5F9; padding: 16px 32px; border-radius: 12px; letter-spacing: 6px; display: inline-block; border: 1px solid #CBD5E1;'>{code}</span>
                    </div>
                    <p style='color: #64748B; font-size: 14px; line-height: 1.5; margin-top: 30px; text-align: center;'>If you didn't request this code, you can safely ignore this email.</p>
                </div>
                <p style='color: #94A3B8; font-size: 12px; text-align: center; margin-top: 20px;'>&copy; {DateTime.Now.Year} MedTrack. All rights reserved.</p>
            </body>
        </html>";

                this.emailSender.SendEmail(userFromDB.Email, "Your Verification Code", emailBody);

                return new CreateUserResponse
                {
                    Message = "Success",
                    StatusCode = System.Net.HttpStatusCode.Created
                };
            }
            return new CreateUserResponse
            {
                Message = "Fail",
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }
    }
}
