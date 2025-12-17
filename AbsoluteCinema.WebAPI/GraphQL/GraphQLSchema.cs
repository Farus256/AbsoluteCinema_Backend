using Microsoft.Extensions.DependencyInjection;
using AbsoluteCinema.WebAPI.GraphQL.Middleware;
using AbsoluteCinema.WebAPI.GraphQL.Queries;
using AbsoluteCinema.WebAPI.GraphQL.Types;

namespace AbsoluteCinema.WebAPI.GraphQL
{
    public static class GraphQLSchema
    {
        public static IServiceCollection AddGraphQLSchema(this IServiceCollection services)
        {
            services
                .AddGraphQLServer()
                .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
                .AddQueryType<MovieQuery>()
                .AddType<MovieType>()
                .AddFiltering()
                .AddSorting()
                .AddProjections();
                //.UseRequest<RequestLoggingMiddleware>();

            return services;
        }
    }
}
