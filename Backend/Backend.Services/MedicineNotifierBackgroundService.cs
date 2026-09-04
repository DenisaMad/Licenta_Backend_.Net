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
                var now = DateTime.Now; // Folosim ora locală a serverului (România)

                var currentHour = now.Hour;
                var currentMinute = now.Minute;

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
                    bool userDocsUpdatedForReset = false;

                    if (user.UserMedicine is not null && user.UserMedicine.Medicines is not null)
                    {
                        bool isNewDayForUser = user.LastResetDate == null || now.Date > user.LastResetDate.Value.Date;

                        if (isNewDayForUser)
                        {
                            foreach (var medicine in user.UserMedicine.Medicines)
                            {
                                medicine.TakenMorning = medicine.CountMorning == 0;
                                medicine.TakenNoon = medicine.CountAfterNon == 0;
                                medicine.TakenEvening = medicine.CountNight == 0;
                            }
                            user.LastResetDate = now.Date;
                            userDocsUpdatedForReset = true;
                        }

                        user.UserNotifications = new List<string>();

                        var validMedicines = user.UserMedicine.Medicines
                          .Where(medicine => medicine.EndDate >= now.Date)
                          .ToList();

                        var medicinesToTakeInMorning = validMedicines.Where(m => m.CountMorning > 0 && !m.TakenMorning).ToList();
                        var medicinesToTakeInEvening = validMedicines.Where(m => m.CountAfterNon > 0 && !m.TakenNoon).ToList();
                        var medicinesToTakeInNight = validMedicines.Where(m => m.CountNight > 0 && !m.TakenEvening).ToList();

                        // Trimitem email EXACT cu 10 minute inainte de ora stabilita (la minutul 50)
                        bool shouldSendEmailMorning = currentHour == 7 && currentMinute == 50;
                        bool shouldSendEmailEvening = currentHour == 15 && currentMinute == 50;
                        bool shouldSendEmailNight = currentHour == 21 && currentMinute == 50;

                        if (shouldSendEmailMorning && medicinesToTakeInMorning.Count > 0)
                        {
                            var timeText = FormatTimeDifference(now, nextMorning);
                            string message = $@"{user.Name ?? user.Email}, it's time to take your morning medicines in {timeText} (at 08:00).";
                            emailService.SendEmail(user.Email, "Medicine Reminder - Morning Dose", message);
                        }
                        if (shouldSendEmailEvening && medicinesToTakeInEvening.Count > 0)
                        {
                            var timeText = FormatTimeDifference(now, nextEvening);
                            string message = $@"{user.Name ?? user.Email}, it's time to take your evening medicines in {timeText} (at 16:00).";
                            emailService.SendEmail(user.Email, "Medicine Reminder - Evening Dose", message);
                        }
                        if (shouldSendEmailNight && medicinesToTakeInNight.Count > 0)
                        {
                            var timeText = FormatTimeDifference(now, nextNight);
                            string message = $@"{user.Name ?? user.Email}, it's time to take your night medicines in {timeText} (at 22:00).";
                            emailService.SendEmail(user.Email, "Medicine Reminder - Night Dose", message);
                        }

                        // Notification Logic
                        if (currentHour >= 8 && currentHour < 16 && medicinesToTakeInMorning.Count > 0)
                        {
                            user.UserNotifications.Add($"You currently have {medicinesToTakeInMorning.Count} active medicines to take this morning.");
                            foreach (var med in medicinesToTakeInMorning)
                                user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountMorning}");
                            notificationsFound = true;
                        }
                        else if (currentHour >= 16 && currentHour < 22 && medicinesToTakeInEvening.Count > 0)
                        {
                            user.UserNotifications.Add($"You currently have {medicinesToTakeInEvening.Count} active medicines to take this afternoon.");
                            foreach (var med in medicinesToTakeInEvening)
                                user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountAfterNon}");
                            notificationsFound = true;
                        }
                        else if (currentHour >= 22 && medicinesToTakeInNight.Count > 0)
                        {
                            user.UserNotifications.Add($"You currently have {medicinesToTakeInNight.Count} active medicines to take tonight.");
                            foreach (var med in medicinesToTakeInNight)
                                user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountNight}");
                            notificationsFound = true;
                        }
                        else
                        {
                            if (currentHour < 8 && medicinesToTakeInMorning.Count > 0)
                            {
                                var timeText = FormatTimeDifference(now, nextMorning);
                                user.UserNotifications.Add($"You have {medicinesToTakeInMorning.Count} medicines to take in {timeText} (at 08:00).");
                                foreach (var med in medicinesToTakeInMorning)
                                    user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountMorning}");
                                notificationsFound = true;
                            }
                            else if (currentHour < 16 && medicinesToTakeInEvening.Count > 0)
                            {
                                var timeText = FormatTimeDifference(now, nextEvening);
                                user.UserNotifications.Add($"You have {medicinesToTakeInEvening.Count} medicines to take in {timeText} (at 16:00).");
                                foreach (var med in medicinesToTakeInEvening)
                                    user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountAfterNon}");
                                notificationsFound = true;
                            }
                            else if (currentHour < 22 && medicinesToTakeInNight.Count > 0)
                            {
                                var timeText = FormatTimeDifference(now, nextNight);
                                user.UserNotifications.Add($"You have {medicinesToTakeInNight.Count} medicines to take in {timeText} (at 22:00).");
                                foreach (var med in medicinesToTakeInNight)
                                    user.UserNotifications.Add($"- You are required to take {med.MedicineName}, Dosage: {med.CountNight}");
                                notificationsFound = true;
                            }
                        }
                    }

                    if (!notificationsFound)
                    {
                        user.UserNotifications.Add("No medicine notifications to send at this time.");
                    }

                    // Always update the document to persist notifications or reset states
                    await userCollection.ReplaceOneAsync(
                        u => u.Id == user.Id,
                        user,
                        cancellationToken: stoppingToken);
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