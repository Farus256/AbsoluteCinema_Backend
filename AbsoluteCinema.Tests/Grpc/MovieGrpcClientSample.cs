using AbsoluteCinema.WebAPI.Grpc;
using Grpc.Net.Client;

namespace AbsoluteCinema.Tests.Grpc;

public static class MovieGrpcClientSample
{
    public static async Task<MovieReply?> TryGetMovieAsync(int id, string grpcAddress, CancellationToken cancellationToken = default)
    {
        // Дозволяємо незашифрований HTTP/2 для локальної розробки.
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        using var channel = GrpcChannel.ForAddress(grpcAddress);
        var client = new MovieGrpc.MovieGrpcClient(channel);

        var response = await client.GetMovieByIdAsync(new GetMovieRequest { Id = id }, cancellationToken: cancellationToken);
        return response;
    }
}

