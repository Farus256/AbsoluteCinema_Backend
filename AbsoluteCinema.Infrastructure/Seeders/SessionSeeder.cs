using AbsoluteCinema.Domain.Entities;
using AbsoluteCinema.Infrastructure.DbContexts;

public class SessionSeeder
{
    public static async Task SeedSessionsAsync(AppDbContext context)
    {
        if (!context.Sessions.Any())
        {
            var random = new Random();
            var halls = context.Halls.ToList();
            var movies = context.Movies.ToList();

            foreach (var movie in movies)
            {
                // Генерация случайного зала
                var randomHall = halls[random.Next(halls.Count)];

                // Генерация случайной даты и времени: от завтра до 7 дней вперёд
                var startTime = DateTime.Today.AddDays(random.Next(1, 7))
                    .AddHours(random.Next(10, 22))     // между 10:00 и 22:00
                    .AddMinutes(15 * random.Next(0, 4)); // кратно 15 минутам

                var session = new Session
                {
                    MovieId = movie.Id,
                    HallId = randomHall.Id,
                    Date = startTime
                };

                context.Sessions.Add(session);
            }

            await context.SaveChangesAsync();
        }
    }
}
