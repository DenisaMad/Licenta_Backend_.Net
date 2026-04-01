using Backend.DataAbstraction;
using Backend.DataAbstraction.Security;
using Backend.Domain.User;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Backend.Services
{
  public sealed class MedicineNotifierBackgroundService : BackgroundService
  {
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public MedicineNotifierBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
      _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var database = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMongoDataBase>();
      var userCollection = database.GetCollection<User>();
      var emailService = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IEmailSender>();

      while (!stoppingToken.IsCancellationRequested)
      {
        var currentDate = DateTime.UtcNow;
        var currentHour = currentDate.Hour;
        var currentMinute = currentDate.Minute;

        int distanceFromCurrentHourToMorningHour = (8 - currentHour + 24) % 24;
        int distanceFromCurrentHourToEveningHour = (16 - currentHour + 24) % 24;
        int distanceFromCurrentHourToNightHour = (22 - currentHour + 24) % 24;

        var users = await userCollection.Find(_ => true).ToListAsync(stoppingToken);

        foreach (var user in users)
        {
          bool notificationsFound = false;

          if (user.UserMedicine is not null)
          {
            user.UserNotifications = new List<string>();

            var validMedicines = user.UserMedicine.Medicines
              .Where(medicine => medicine.EndDate >= currentDate.Date)
              .ToList();

            bool shouldSendEmailMorning = distanceFromCurrentHourToMorningHour == 0 && currentMinute >= 50;
            bool shouldSendEmailEvening = distanceFromCurrentHourToEveningHour == 0 && currentMinute >= 50;
            bool shouldSendEmailNight = distanceFromCurrentHourToNightHour == 0 && currentMinute >= 50;

            if (distanceFromCurrentHourToMorningHour >= 0 &&
                distanceFromCurrentHourToMorningHour <= distanceFromCurrentHourToEveningHour &&
                distanceFromCurrentHourToMorningHour <= distanceFromCurrentHourToNightHour)
            {
              string message =
                $@"{user.Name ?? user.Email}, it's time to take your morning medicines in {distanceFromCurrentHourToMorningHour} hours (at 08:00).";

              if (shouldSendEmailMorning)
              {
                emailService.SendEmail(user.Email, "Medicine Reminder - Morning Dose", message);
              }

              var medicinesToTakeInMorning = validMedicines
                .Where(medicine => medicine.CountMorning > 0)
                .ToList();

              user.UserNotifications.Add(
                $"You have {medicinesToTakeInMorning.Count} medicines to take in {distanceFromCurrentHourToMorningHour} hours (at 08:00).");

              foreach (var med in medicinesToTakeInMorning)
              {
                user.UserNotifications.Add(
                  $"- You are required to take {med.MedicineName}, Dosage: {med.CountMorning}");
              }

              await userCollection.ReplaceOneAsync(
                u => u.Id == user.Id,
                user,
                cancellationToken: stoppingToken);

              notificationsFound = true;
            }
            else if (distanceFromCurrentHourToEveningHour >= 0 &&
                     distanceFromCurrentHourToEveningHour <= distanceFromCurrentHourToMorningHour &&
                     distanceFromCurrentHourToEveningHour <= distanceFromCurrentHourToNightHour)
            {
              string message =
                $@"{user.Name ?? user.Email}, it's time to take your evening medicines in {distanceFromCurrentHourToEveningHour} hours (at 16:00).";

              if (shouldSendEmailEvening)
              {
                emailService.SendEmail(user.Email, "Medicine Reminder - Evening Dose", message);
              }

              var medicinesToTakeInEvening = validMedicines
                .Where(medicine => medicine.CountAfterNon > 0)
                .ToList();

              user.UserNotifications.Add(
                $"You have {medicinesToTakeInEvening.Count} medicines to take in {distanceFromCurrentHourToEveningHour} hours (at 16:00).");

              foreach (var med in medicinesToTakeInEvening)
              {
                user.UserNotifications.Add(
                  $"- You are required to take {med.MedicineName}, Dosage: {med.CountAfterNon}");
              }

              await userCollection.ReplaceOneAsync(
                u => u.Id == user.Id,
                user,
                cancellationToken: stoppingToken);

              notificationsFound = true;
            }
            else if (distanceFromCurrentHourToNightHour >= 0 &&
                     distanceFromCurrentHourToNightHour <= distanceFromCurrentHourToMorningHour &&
                     distanceFromCurrentHourToNightHour <= distanceFromCurrentHourToEveningHour)
            {
              string message =
                $@"{user.Name ?? user.Email}, it's time to take your night medicines in {distanceFromCurrentHourToNightHour} hours (at 22:00).";

              if (shouldSendEmailNight)
              {
                emailService.SendEmail(user.Email, "Medicine Reminder - Night Dose", message);
              }

              var medicinesToTakeInNight = validMedicines
                .Where(medicine => medicine.CountNight > 0)
                .ToList();

              user.UserNotifications.Add(
                $"You have {medicinesToTakeInNight.Count} medicines to take in {distanceFromCurrentHourToNightHour} hours (at 22:00).");

              foreach (var med in medicinesToTakeInNight)
              {
                user.UserNotifications.Add(
                  $"- You are required to take {med.MedicineName}, Dosage: {med.CountNight}");
              }

              await userCollection.ReplaceOneAsync(
                u => u.Id == user.Id,
                user,
                cancellationToken: stoppingToken);

              notificationsFound = true;
            }
          }

          if (!notificationsFound)
          {
            user.UserNotifications.Add("No medicine notifications to send at this time.");

            await userCollection.ReplaceOneAsync(
              u => u.Id == user.Id,
              user,
              cancellationToken: stoppingToken);
          }
        }

        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
      }
    }
  }
}