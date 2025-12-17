using AbsoluteCinema.Application.Contracts;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;

namespace AbsoluteCinema.WebAPI.GraphQL.Queries
{
    public class MovieQuery
    {
        [GraphQLName("movies")]
        [GraphQLDescription("Gets all movies with included related data")]
        public async Task<IEnumerable<AbsoluteCinema.Application.DTO.Entities.MovieDto>> GetMovies(
            [Service] IMovieService movieService,
            [Service] ILogger<MovieQuery> logger)
        {
            try
            {
                return await movieService.GetAllMoviesWithIncludeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting movies");
                throw;
            }
        }

        [GraphQLName("movie")]
        [GraphQLDescription("Gets a movie by its ID")]
        public async Task<AbsoluteCinema.Application.DTO.Entities.MovieDto?> GetMovieById(
            [GraphQLName("id")] int id,
            [Service] IMovieService movieService,
            [Service] ILogger<MovieQuery> logger)
        {
            try
            {
                return await movieService.GetMovieByIdAsync(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error while getting movie with ID {id}");
                throw;
            }
        }
    }
}
