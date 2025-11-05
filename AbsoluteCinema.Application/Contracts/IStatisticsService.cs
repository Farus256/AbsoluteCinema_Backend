using AbsoluteCinema.Application.DTO.AuthDTO.StatisticsDto;

public interface IStatisticsService
{
    Task<double> GetRevenueAsync(DateTime? startDate, DateTime? endDate);
    Task<IEnumerable<TopMovieDto>> GetTopMoviesByPeriodAsync(DateTime startDate, DateTime endDate, int quantityOfMoviesInTop);
    Task<IEnumerable<HallDto>> GetTopHallsByPeriodAsync(DateTime startDate, DateTime endDate, string orderDir = "desc");
    Task<IEnumerable<WeekdayDto>> GetBusiestDaysAsync();
}
