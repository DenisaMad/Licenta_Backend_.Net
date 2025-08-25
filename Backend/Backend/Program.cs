using FluentValidation.AspNetCore;
using InterviewCrusherAdmin;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        Assembly[] assemblies = builder.Services.RegisterServices();
        builder.Services.AddMediatR(x=>x.RegisterServicesFromAssemblies(assemblies));
        builder.Services.AddFluentValidation(fv =>
        {
            fv.RegisterValidatorsFromAssemblies(assemblies);
            fv.AutomaticValidationEnabled = true;
        });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}