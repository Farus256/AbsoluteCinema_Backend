using AbsoluteCinema.Application.Contracts;
using AbsoluteCinema.WebAPI.Grpc;
using Grpc.Core;

namespace AbsoluteCinema.WebAPI.Grpc.Services;

public sealed class MovieGrpcService(IMovieService movieService) : MovieGrpc.MovieGrpcBase
{
    private readonly IMovieService _movieService = movieService;

    public override async Task<MovieReply> GetMovieById(GetMovieRequest request, ServerCallContext context)
    {
        var movie = await _movieService.GetMovieByIdAsync(request.Id);
        if (movie is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Movie {request.Id} not found"));
        }

        return new MovieReply
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Discription ?? string.Empty,
            Score = movie.Score ?? 0,
            Adult = movie.Adult,
            PosterPath = movie.PosterPath ?? string.Empty,
            Language = movie.Language.ToString(),
            ReleaseDate = movie.ReleaseDate?.ToString("O") ?? string.Empty
        };
    }
}

