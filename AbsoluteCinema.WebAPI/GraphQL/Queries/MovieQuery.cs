using AbsoluteCinema.Application.Contracts;
using AbsoluteCinema.Application.DTO.Entities;
using AbsoluteCinema.Application.DTO.MoviesDTO;
using AbsoluteCinema.Domain.Entities;
using AbsoluteCinema.Domain.Interfaces;
using HotChocolate.Types;
using Microsoft.Extensions.Logging;

namespace AbsoluteCinema.WebAPI.GraphQL.Queries
{

    public class MovieQuery
    {
        [GraphQLName("moviesPaged")]
        public async Task<IEnumerable<MovieDto>> GetMoviesPaged(
            int page,
            int pageSize,
            [Service] IMovieService movieService)
        {
            return await movieService.GetAllMoviesAsync(new GetAllMoviesDto
            {
                Page = page,
                PageSize = pageSize,
                OrderByProperty = "Id",
                OrderDirection = "asc"
            });
        }

        [GraphQLName("movieCards")]
        public async Task<IEnumerable<MovieCardDto>> GetMovieCards([Service] IUnitOfWork uow)
        {
            var movies = await uow.Repository<Movie>().GetAllAsync(); // без include

            return movies.Select(m => new MovieCardDto
            {
                Id = m.Id,
                Title = m.Title,
                PosterPath = m.PosterPath
            });
        }
    }
}
