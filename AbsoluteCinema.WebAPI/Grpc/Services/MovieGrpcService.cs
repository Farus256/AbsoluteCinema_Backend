using AbsoluteCinema.Application.Contracts;
using AbsoluteCinema.WebAPI.Grpc;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace AbsoluteCinema.WebAPI.Grpc.Services;

public sealed class MovieGrpcService(IMovieService movieService, ILogger<MovieGrpcService> logger) : MovieGrpc.MovieGrpcBase
{
    private readonly IMovieService _movieService = movieService;
    private readonly ILogger<MovieGrpcService> _logger = logger;

    public override async Task<MovieReply> GetMovieById(GetMovieRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("gRPC GetMovieById called with Id {Id}", request.Id);

            var movie = await _movieService.GetMovieByIdAsync(request.Id);

            if (movie is null)
            {
                _logger.LogWarning("Movie {Id} not found in GetMovieById", request.Id);
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
        catch (RpcException)
        {
            // Already a well-defined gRPC error, just propagate
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GetMovieById for Id {Id}", request.Id);
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error while getting movie."));
        }
    }
}
