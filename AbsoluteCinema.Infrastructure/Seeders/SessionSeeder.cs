using AbsoluteCinema.Domain.Entities;
using AbsoluteCinema.Infrastructure.DbContexts;

using Microsoft.Extensions.Logging;

public class SessionSeeder
{
    public static async Task SeedSessionsAsync(AppDbContext context, ILogger? logger = null)
    {
        var existingSessionsCount = context.Sessions.Count();
        logger?.LogInformation($"SessionSeeder: Поточна кількість сесій в базі: {existingSessionsCount}");
        
        if (!context.Sessions.Any())
        {
            var random = new Random();
            var halls = context.Halls.ToList();
            var movies = context.Movies.ToList();

            logger?.LogInformation($"SessionSeeder: Знайдено {halls.Count} залів та {movies.Count} фільмів");

            if (halls.Count == 0)
            {
                logger?.LogWarning("SessionSeeder: Немає залів в базі! Сесії не можуть бути створені.");
                return;
            }

            if (movies.Count == 0)
            {
                logger?.LogWarning("SessionSeeder: Немає фільмів в базі! Сесії не можуть бути створені.");
                return;
            }

            int totalSessionsCreated = 0;

            foreach (var movie in movies)
            {
                int sessionsCount = random.Next(2, 5);
                
                for (int i = 0; i < sessionsCount; i++)
                {
                    var randomHall = halls[random.Next(halls.Count)];
                    var startTime = DateTime.Today.AddDays(random.Next(1, 8))
                        .AddHours(random.Next(10, 22))
                        .AddMinutes(15 * random.Next(0, 4));

                    var session = new Session
                    {
                        MovieId = movie.Id,
                        HallId = randomHall.Id,
                        Date = startTime
                    };

                    context.Sessions.Add(session);
                    totalSessionsCreated++;
                }
            }

            await context.SaveChangesAsync();
            logger?.LogInformation($"SessionSeeder: Створено {totalSessionsCreated} сесій для {movies.Count} фільмів");
        }
        else
        {
            logger?.LogInformation("SessionSeeder: Сесії вже існують в базі, пропускаємо створення");
        }
    }
}
