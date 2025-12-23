using Backend.DataAbstraction;
using Backend.DataAbstraction.BearerTokens;
using Backend.DataAbstraction.Database;
using Backend.DataAbstraction.Security;
using Backend.Database;
using Backend.Services.Database;
using Backend.Services.Security;
using FluentValidation.AspNetCore;
using InterviewCrusherAdmin;
using System.Reflection;

internal class Program
{
  private static void Main(string[] args)
  {
        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins, policy =>
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });
        builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    Assembly[] assemblies = builder.Services.RegisterServices();
    builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblies(assemblies));
    builder.Services.AddFluentValidation(fv =>
    {
      fv.RegisterValidatorsFromAssemblies(assemblies);
      fv.AutomaticValidationEnabled = true;
    });

        builder.Services.AddScoped<IHashingSettings, HashingSettings>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var hashingService = new HashingSettings();
            config.GetSection("HashingSettings").Bind(hashingService);
            return hashingService;
        });
        builder.Services.AddScoped<IHashingService, HashingService>();
   
        builder.Services.AddScoped<IAuthentificationService, AuthentificationService>();
        builder.Services.AddSingleton<IDatabaseSettings, DatabaseSettings>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var databaseService = new DatabaseSettings();
            config.GetSection("DatabaseSettings").Bind(databaseService);
            return databaseService;
        });

        builder.Services.AddSingleton<IMongoDataBase, MongoDataBase>();

        builder.Services.AddScoped<IEmailConfig, EmailConfig>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var emailConfig = new EmailConfig();
            config.GetSection("EmailConfig").Bind(emailConfig);
            return emailConfig;
        });

        builder.Services.AddScoped<IEmailSender, EmailSender>();

        builder.Services.AddScoped<IAuthTokensService, AuthTokensService>();

        builder.Services.AddScoped<IBearerTokenService, BearerTokenService>();



        var app = builder.Build();

        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

        app.UseCors(MyAllowSpecificOrigins);
        app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
  }
}