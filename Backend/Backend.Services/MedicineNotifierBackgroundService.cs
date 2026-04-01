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
        var now = DateTime.UtcNow;
        var currentHour = now.Hour;
        var currentMinute = now.Minute;

        int distanceFromCurrentHourToMorningHour = (8 - currentHour + 24) % 24;
        int distanceFromCurrentHourToEveningHour = (16 - currentHour + 24) % 24;
        int distanceFromCurrentHourToNightHour = (22 - currentHour + 24) % 24;

        DateTime nextMorning = now.Date.AddHours(8);
        if (nextMorning <= now) nextMorning = nextMorning.AddDays(1);

        DateTime nextEvening = now.Date.AddHours(16);
        if (nextEvening <= now) nextEvening = nextEvening.AddDays(1);

        DateTime nextNight = now.Date.AddHours(22);
        if (nextNight <= now) nextNight = nextNight.AddDays(1);

        var users = await userCollection.Find(_ => true).ToListAsync(stoppingToken);

        foreach (var user in users)
        {
          bool notificationsFound = false;

          if (user.UserMedicine is not null)
          {
            user.UserNotifications = new List<string>();

            var validMedicines = user.UserMedicine.Medicines
              .Where(medicine => medicine.EndDate >= now.Date)
              .ToList();

            bool shouldSendEmailMorning = distanceFromCurrentHourToMorningHour == 0 && currentMinute >= 50;
            bool shouldSendEmailEvening = distanceFromCurrentHourToEveningHour == 0 && currentMinute >= 50;
            bool shouldSendEmailNight = distanceFromCurrentHourToNightHour == 0 && currentMinute >= 50;

            if (distanceFromCurrentHourToMorningHour >= 0 &&
                distanceFromCurrentHourToMorningHour <= distanceFromCurrentHourToEveningHour &&
                distanceFromCurrentHourToMorningHour <= distanceFromCurrentHourToNightHour)
            {
              var timeText = FormatTimeDifference(now, nextMorning);

              string message =
                $@"{user.Name ?? user.Email}, it's time to take your morning medicines in {timeText} (at 08:00).";

              if (shouldSendEmailMorning)
              {
                emailService.SendEmail(user.Email, "Medicine Reminder - Morning Dose", message);
              }

              var medicinesToTakeInMorning = validMedicines
                .Where(medicine => medicine.CountMorning > 0)
                .ToList();

              user.UserNotifications.Add(
                $"You have {medicinesToTakeInMorning.Count} medicines to take in {timeText} (at 08:00).");

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
              var timeText = FormatTimeDifference(now, nextEvening);

              string message =
                $@"{user.Name ?? user.Email}, it's time to take your evening medicines in {timeText} (at 16:00).";

              if (shouldSendEmailEvening)
              {
                emailService.SendEmail(user.Email, "Medicine Reminder - Evening Dose", message);
              }

              var medicinesToTakeInEvening = validMedicines
                .Where(medicine => medicine.CountAfterNon > 0)
                .ToList();

              user.UserNotifications.Add(
                $"You have {medicinesToTakeInEvening.Count} medicines to take in {timeText} (at 16:00).");

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
              var timeText = FormatTimeDifference(now, nextNight);

              string message =
                $@"{user.Name ?? user.Email}, it's time to take your night medicines in {timeText} (at 22:00).";

              if (shouldSendEmailNight)
              {
                emailService.SendEmail(user.Email, "Medicine Reminder - Night Dose", message);
              }

              var medicinesToTakeInNight = validMedicines
                .Where(medicine => medicine.CountNight > 0)
                .ToList();

              user.UserNotifications.Add(
                $"You have {medicinesToTakeInNight.Count} medicines to take in {timeText} (at 22:00).");

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

    private static string FormatTimeDifference(DateTime now, DateTime target)
    {
      var diff = target - now;

      int hours = (int)diff.TotalHours;
      int minutes = diff.Minutes;

      return $"{hours} hours and {minutes} minutes";
    }
  }
}