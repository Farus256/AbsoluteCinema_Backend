using AbsoluteCinema.Application.Movies.Queries;
using HotChocolate.Types;

namespace AbsoluteCinema.WebAPI.GraphQL.Types
{
    public class MovieResponseType : ObjectType<GetMovieListQueryResponse>
    {
        protected override void Configure(IObjectTypeDescriptor<GetMovieListQueryResponse> descriptor)
        {
            descriptor.Description("Response containing a list of movies");
            
            descriptor
                .Field(r => r.Movies)
                .Description("The list of movies")
                .Type<ListType<MovieType>>();
        }
    }
}
