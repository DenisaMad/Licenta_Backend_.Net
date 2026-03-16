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
        int distanceFromCurrentHourToMorningHour = (8 - currentHour + 24) % 24;
        int distanceFromCurrentHourToEveningHour = (16 - currentHour + 24) % 24;
        int distanceFromCurrentHourToNightHour = (22 - currentHour + 24) % 24;
        var users = await userCollection.Find(_ => true).ToListAsync(stoppingToken);
        bool notificationsFound = false;
        foreach (var user in users)
        {
          if (user.UserMedicine is not null)
          {
            user.UserNotifications = new List<string>();
            var distanceInHours = user.UserMedicine.Medicines.Where(
              medicine => medicine.EndDate >= currentDate.Date).ToList();
            if (distanceFromCurrentHourToMorningHour > 0 && distanceFromCurrentHourToMorningHour < distanceFromCurrentHourToEveningHour && distanceFromCurrentHourToMorningHour < distanceFromCurrentHourToNightHour && distanceFromCurrentHourToMorningHour < 2)
            {
              string message = $@"{user.Name ?? user.Email}, it's time to take your morning medicines in {distanceFromCurrentHourToMorningHour} hours.";
              emailService.SendEmail(user.Email, "Medicine Reminder - Morning Dose", message);
              var medicinesToTakeInMorning = user.UserMedicine.Medicines.Where(
                medicine => medicine.EndDate >= currentDate.Date && medicine.CountMorning > 0).ToList();
              user.UserNotifications.Add($"You have {medicinesToTakeInMorning.Count} medicines to take in the morning.");
              foreach(var med in medicinesToTakeInMorning)
              {
                user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountMorning}");
              }
              await userCollection.ReplaceOneAsync(
                u => u.Id == user.Id,
                user,
                cancellationToken: stoppingToken);
              notificationsFound = true;
            }
            else if (distanceFromCurrentHourToEveningHour > 0 && distanceFromCurrentHourToEveningHour < distanceFromCurrentHourToMorningHour && distanceFromCurrentHourToEveningHour < distanceFromCurrentHourToNightHour && distanceFromCurrentHourToEveningHour < 2)
            {
              string message = $@"{user.Name ?? user.Email}, it's time to take your evening medicines in {distanceFromCurrentHourToEveningHour} hours.";
              emailService.SendEmail(user.Email, "Medicine Reminder - Evening Dose", message);
              var medicinesToTakeInEvening = user.UserMedicine.Medicines.Where(
                medicine => medicine.EndDate >= currentDate.Date && medicine.CountAfterNon > 0).ToList();
              var eveningMedicineNotifications = new UserNotification
              {
                Notifications = medicinesToTakeInEvening.ToList()
              };
              string notificationMessage= $"You have {eveningMedicineNotifications.Notifications.Count} medicines to take in the evening.";
              user.UserNotifications.Add(notificationMessage);
              foreach(var med in eveningMedicineNotifications.Notifications)
              {
                user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountAfterNon}");
              }
              await userCollection.ReplaceOneAsync(
                u => u.Id == user.Id,
                user,
                cancellationToken: stoppingToken);
              notificationsFound = true;
            }
            else if (distanceFromCurrentHourToNightHour > 0 && distanceFromCurrentHourToNightHour < distanceFromCurrentHourToMorningHour && distanceFromCurrentHourToNightHour < distanceFromCurrentHourToEveningHour && distanceFromCurrentHourToNightHour < 2)
            {
              string message = $@"{user.Name ?? user.Email}, it's time to take your night medicines in {distanceFromCurrentHourToNightHour} hours.";
              emailService.SendEmail(user.Email, "Medicine Reminder - Night Dose", message);
              var medicinesToTakeInNight = user.UserMedicine.Medicines.Where(
                medicine => medicine.EndDate >= currentDate.Date && medicine.CountNight > 0).ToList();
              user.UserNotifications.Add($"You have {medicinesToTakeInNight.Count} medicines to take at night.");
              foreach(var med in medicinesToTakeInNight)
              {
                user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountNight}");
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
       
        await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
      }
    }
  }
}
