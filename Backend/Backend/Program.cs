using Backend.DataAbstraction;
using Backend.DataAbstraction.BearerTokens;
using Backend.DataAbstraction.Database;
using Backend.DataAbstraction.Security;
using Backend.Database;
using Backend.Services;
using Backend.Services.Database;
using Backend.Services.Security;
using FluentValidation.AspNetCore;
using InterviewCrusherAdmin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

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
        builder.Services.AddScoped<IDjangoService, DjangoService>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
        });
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
        builder.Services.AddScoped<IDjangoService, DjangoService>();
        builder.Services.AddHostedService<MedicineNotifierBackgroundService>();
        builder.Services.AddSingleton<Backend.DataAbstraction.Security.IRsaKeyProvider, Backend.Services.Security.RsaKeyProvider>();


        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bahdoiajfncajnfhoijnaoifanif131jdangsfdsbahdoiajfncajnfhoijnaoifanif131jdangsfdsbahdoiajfncajnfhoijnaoifanif131jdangsfds")),
                    ValidateIssuer = true,
                    ValidIssuer = "your_issuer",
                    ValidateAudience = true,
                    ValidAudience = "your_audience",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(MyAllowSpecificOrigins);
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseMiddleware<Backend.Middleware.CustomAuthorizationMiddleware>();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}